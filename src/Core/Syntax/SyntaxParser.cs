using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Execution.Operations;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Diagnostic;

namespace Core.Syntax
{
    public class SyntaxParser : ISyntaxParser
    {
        private readonly Dictionary<TokenType, OperationPrecedence> binaryOperatorPrecedences;
        private readonly Dictionary<TokenType, OperationPrecedence> unaryOperatorPrecedences;
        private readonly Dictionary<TokenType, TokenType> compoundOperators = new()
        {
            [TokenType.PlusEquals] = TokenType.Plus,
            [TokenType.MinusEquals] = TokenType.Minus,
            [TokenType.StarEquals] = TokenType.Star,
            [TokenType.SlashEquals] = TokenType.Slash
        };

        private ImmutableArray<SyntaxToken> tokens;
        private DiagnosticBag diagnostic;
        private int position;

        public SyntaxParser(IEnumerable<BinaryOperation> binaryOperations, IEnumerable<UnaryOperation> unaryOperations)
        {
            binaryOperatorPrecedences = binaryOperations
                .ToDictionary(operation => operation.Operator, operation => operation.Precedence);
            
            unaryOperatorPrecedences = unaryOperations
                .ToDictionary(operation => operation.Operator, operation => operation.Precedence);
        }
        
        private SyntaxToken Current => Peek(0);
        
        private SyntaxToken Lookahead => Peek(1);

        private bool IsCurrentIdentifierWithEquals =>
            Current.Type == TokenType.Identifier && Lookahead.Type == TokenType.Equals;
        
        public TranslationState<SyntaxTree> Parse(IEnumerable<SyntaxToken> syntaxTokens)
        {
            position = 0;
            diagnostic = new DiagnosticBag();
            tokens = syntaxTokens
                .Where(token => token.Type != TokenType.Space && token.Type != TokenType.NewLine)
                .ToImmutableArray();
            
            var members = ImmutableArray.CreateBuilder<SyntaxNode>();
            while (Current.Type != TokenType.Eof)
            {
                var startToken = Current;
                
                var member = ParseStatement();
                members.Add(member);

                if (Current == startToken)
                    NextToken();
            }

            var syntaxTree = new SyntaxTree(members.ToImmutable());
            return new TranslationState<SyntaxTree>(syntaxTree, diagnostic);
        }
        
        private SyntaxToken Peek(int offset)
        {
            var index = position + offset;
            
            return index >= tokens.Length 
                ? tokens.Last() 
                : tokens[index];
        }

        private SyntaxToken NextToken()
        {
            var current = Current;

            position++;

            return current;
        }
        
        private SyntaxToken MatchToken(TokenType expected)
        {
            if (Current.Type == expected)
                return NextToken();

            diagnostic.AddError(Current.Location, $"Expected '{expected}'");
            
            return new SyntaxToken(expected, null, Current.Location);
        }

        private Statement ParseStatement()
        {
            return Current.Type switch
            {
                TokenType.OpenBrace => ParseBlockStatement(),
                TokenType.IfKeyword => ParseIfStatement(),
                TokenType.WhileKeyword => ParseWhileStatement(),
                TokenType.ForKeyword => ParseForStatement(),
                TokenType.DefKeyword => ParseFunctionDeclarationStatement(),
                TokenType.ReturnKeyword => ParseReturnStatement(),
                TokenType.BreakKeyword => ParseBreakStatement(),
                TokenType.ContinueKeyword => ParseContinueStatement(),
                _ => ParseExpressionStatement()
            };
        }

        private FunctionDeclarationStatement ParseFunctionDeclarationStatement()
        {
            var keyword = MatchToken(TokenType.DefKeyword);
            var identifier = MatchToken(TokenType.Identifier);
            var openParenthesis = MatchToken(TokenType.OpenParenthesis);
            var positionParameters = ParsePositionParameters();
            var defaultParameters = ParseDefaultParameters();
            var closeParenthesis = MatchToken(TokenType.CloseParenthesis);
            var block = ParseBlockStatement();

            return new FunctionDeclarationStatement(
                keyword, identifier, openParenthesis, positionParameters, defaultParameters,
                closeParenthesis, block
            );
        }

        private ImmutableArray<SyntaxToken> ParsePositionParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<SyntaxToken>();
            
            while (!IsCurrentIdentifierWithEquals 
                   && Current.Type != TokenType.CloseParenthesis && Current.Type != TokenType.Eof)
            {
                var parameter = MatchToken(TokenType.Identifier);
                parameters.Add(parameter);
                
                if (Current.Type != TokenType.Comma)
                    break;

                MatchToken(TokenType.Comma);
            }

            return parameters.ToImmutable();
        }

        private ImmutableArray<SyntaxDefaultParameter> ParseDefaultParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<SyntaxDefaultParameter>();
            
            while (IsCurrentIdentifierWithEquals)
            {
                var name = NextToken();
                var equals = NextToken();
                var expression = ParseBinaryExpression();

                var parameter = new SyntaxDefaultParameter(name, equals, expression);
                parameters.Add(parameter);
                
                if (Current.Type != TokenType.Comma)
                    break;

                MatchToken(TokenType.Comma);
            }

            return parameters.ToImmutable();
        }

        private ReturnStatement ParseReturnStatement()
        {
            var keyword = MatchToken(TokenType.ReturnKeyword);
            var expression = Current.Type == TokenType.Semicolon 
                ? null 
                : ParseExpression();
            var semicolon = MatchToken(TokenType.Semicolon);

            return new ReturnStatement(keyword, expression, semicolon);
        }

        private BreakStatement ParseBreakStatement()
        {
            var keyword = MatchToken(TokenType.BreakKeyword);
            var semicolon = MatchToken(TokenType.Semicolon);

            return new BreakStatement(keyword, semicolon);
        }
        
        private ContinueStatement ParseContinueStatement()
        {
            var keyword = MatchToken(TokenType.ContinueKeyword);
            var semicolon = MatchToken(TokenType.Semicolon);

            return new ContinueStatement(keyword, semicolon);
        }

        private ForStatement ParseForStatement()
        {
            var keyword = MatchToken(TokenType.ForKeyword);
            var openParenthesis = MatchToken(TokenType.OpenParenthesis);
            var initializers = ParseSeparatedExpressions(TokenType.Semicolon);
            var firstSemicolon = MatchToken(TokenType.Semicolon);
            var condition = ParseExpression();
            var secondSemicolon = MatchToken(TokenType.Semicolon);
            var iterators = ParseSeparatedExpressions(TokenType.CloseParenthesis);
            var closeParenthesis = MatchToken(TokenType.CloseParenthesis);
            var body = ParseStatement();

            return new ForStatement(
                keyword, 
                openParenthesis, 
                initializers, 
                firstSemicolon, 
                condition, 
                secondSemicolon,
                iterators, 
                closeParenthesis,
                body
            );
        }

        private WhileStatement ParseWhileStatement()
        {
            var keyword = MatchToken(TokenType.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();

            return new WhileStatement(keyword, condition, body);
        }
        
        private IfStatement ParseIfStatement()
        {
            var keyword = MatchToken(TokenType.IfKeyword);
            var condition = ParseExpression();
            var thenStatement = ParseStatement();
            var elseClause = Current.Type == TokenType.ElseKeyword
                ? ParseElseClause()
                : null;

            return new IfStatement(keyword, condition, thenStatement, elseClause);
        }

        private ElseClause ParseElseClause()
        {
            var keyword = MatchToken(TokenType.ElseKeyword);
            var elseStatement = ParseStatement();

            return new ElseClause(keyword, elseStatement);
        }

        private BlockStatement ParseBlockStatement()
        {
            var openBrace = MatchToken(TokenType.OpenBrace);

            var statements = ImmutableArray.CreateBuilder<Statement>();
            while (Current.Type != TokenType.CloseBrace && Current.Type != TokenType.Eof)
            {
                var startToken = Current;
                
                var statement = ParseStatement();
                statements.Add(statement);

                if (Current == startToken)
                    NextToken();
            }

            var closeBrace = MatchToken(TokenType.CloseBrace);

            return new BlockStatement(openBrace, statements.ToImmutable(), closeBrace);
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var expression = ParseExpression();
            var semicolon = MatchToken(TokenType.Semicolon);

            return new ExpressionStatement(expression, semicolon);
        }

        private Expression ParseExpression()
        {
            var expression = ParseBinaryExpression();

            if (compoundOperators.ContainsKey(Current.Type))
            {
                switch (expression)
                {
                    case VariableExpression variableExpression:
                        return ContinueWithCompoundVariableAssignmentExpression(variableExpression);
                        
                    case IndexAccessExpression indexAccessExpression:
                        return ContinueWithCompoundIndexAssignmentExpression(indexAccessExpression);
                }
            }

            if (Current.Type == TokenType.Equals)
            {
                switch (expression)
                {
                    case VariableExpression variableExpression:
                        return ContinueWithVariableAssignmentExpression(variableExpression);
                        
                    case IndexAccessExpression indexAccessExpression:
                        return ContinueWithIndexAssignmentExpression(indexAccessExpression);
                }
            }

            return ParseBinaryExpression(initializedLeft: expression);
        }

        private IndexAssignmentExpression ContinueWithIndexAssignmentExpression(IndexAccessExpression indexAccessExpression)
        {
            var equals = MatchToken(TokenType.Equals);
            var value = ParseExpression();

            return new IndexAssignmentExpression(
                indexAccessExpression.IndexedExpression, indexAccessExpression.Index, equals, value
            );
        }

        private IndexAssignmentExpression ContinueWithCompoundIndexAssignmentExpression(IndexAccessExpression indexAccessExpression)
        {
            var (compoundOperator, expression) = ContinueCompoundAssignmentExpression(indexAccessExpression);
            
            return new IndexAssignmentExpression(
                indexAccessExpression.IndexedExpression, indexAccessExpression.Index, compoundOperator, expression
            );
        }

        private VariableAssignmentExpression ContinueWithCompoundVariableAssignmentExpression(VariableExpression variableExpression)
        {
            var (compoundOperator, expression) = ContinueCompoundAssignmentExpression(variableExpression);
            
            return new VariableAssignmentExpression(variableExpression.Identifier, compoundOperator, expression);
        }

        private (SyntaxToken compoundOperator, BinaryExpression expression) ContinueCompoundAssignmentExpression(Expression leftExpression)
        {
            var compoundOperator = NextToken();
            var singleOperatorType = compoundOperators[compoundOperator.Type];

            var singleOperator = new SyntaxToken(singleOperatorType, compoundOperator.Text, compoundOperator.Location);
            var rightExpression = ParseExpression();

            return (compoundOperator, new BinaryExpression(leftExpression, singleOperator, rightExpression));
        }

        private VariableAssignmentExpression ContinueWithVariableAssignmentExpression(VariableExpression variable)
        {
            var equals = MatchToken(TokenType.Equals);
            var expression = ParseExpression();

            return new VariableAssignmentExpression(variable.Identifier, equals, expression);
        }

        private Expression ParseBinaryExpression(
            Expression initializedLeft = null,
            OperationPrecedence previousPrecedence = OperationPrecedence.None)
        {
            var left = initializedLeft ?? (unaryOperatorPrecedences.ContainsKey(Current.Type)
                ? ParseUnaryExpression()
                : ParsePrimaryExpression());

            while (true)
            {
                if (!binaryOperatorPrecedences.TryGetValue(Current.Type, out var precedence)
                    || precedence <= previousPrecedence)
                {
                    break;
                }

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(previousPrecedence: precedence);

                left = new BinaryExpression(left, operatorToken, right);
            }

            return left;
        }

        private UnaryExpression ParseUnaryExpression()
        {
            var op = NextToken();
            var expression = ParseBinaryExpression(previousPrecedence: unaryOperatorPrecedences[op.Type]);

            return new UnaryExpression(op, expression);
        }

        private Expression ParsePrimaryExpression()
        {
            var primary = Current.Type switch
            {
                TokenType.OpenParenthesis => ParseParenthesizedExpression(),
                TokenType.OpenBracket => ParseArrayExpression(),
                TokenType.TrueKeyword or TokenType.FalseKeyword => ParseBooleanExpression(),
                TokenType.NullKeyword => ParseNullExpression(),
                TokenType.Number => ParseNumberExpression(),
                TokenType.String => ParseStringExpression(),
                _ => ParseVariableOrCallExpression()
            };

            if (Current.Type == TokenType.OpenBracket)
                return ContinueWithIndexAccessExpression(primary);

            return primary;
        }

        private Expression ContinueWithIndexAccessExpression(Expression parent)
        {
            do
            {
                var index = ParseSyntaxIndex();

                parent = new IndexAccessExpression(parent, index);
                
            } while (Current.Type == TokenType.OpenBracket);

            return parent;
        }

        private ParenthesizedExpression ParseParenthesizedExpression()
        {
            var open = MatchToken(TokenType.OpenParenthesis);
            var expression = ParseExpression();
            var close = MatchToken(TokenType.CloseParenthesis);

            return new ParenthesizedExpression(open, expression, close);
        }

        private NumberExpression ParseNumberExpression()
        {
            var numberToken = MatchToken(TokenType.Number);
            var value = Convert.ToDouble(numberToken.Text, CultureInfo.InvariantCulture);
            
            return new NumberExpression(value);
        }

        private BooleanExpression ParseBooleanExpression()
        {
            var value = Current.Type == TokenType.TrueKeyword;
            _ = value ? MatchToken(TokenType.TrueKeyword) : MatchToken(TokenType.FalseKeyword);

            return new BooleanExpression(value);
        }

        private StringExpression ParseStringExpression()
        {
            var token = MatchToken(TokenType.String);

            return new StringExpression(token.Text);
        }

        private NullExpression ParseNullExpression()
        {
            MatchToken(TokenType.NullKeyword);

            return new NullExpression();
        }

        private Expression ParseVariableOrCallExpression()
        {
            if (Current.Type == TokenType.Identifier && Lookahead.Type == TokenType.OpenParenthesis)
                return ParseCallExpression();

            return ParseVariableExpression();
        }

        private FunctionCallExpression ParseCallExpression()
        {
            var name = MatchToken(TokenType.Identifier);
            var openParenthesis = MatchToken(TokenType.OpenParenthesis);
            var arguments = ParseSeparatedExpressions(TokenType.CloseParenthesis);
            var closeParenthesis = MatchToken(TokenType.CloseParenthesis);

            return new FunctionCallExpression(name, openParenthesis, arguments, closeParenthesis);
        }

        private VariableExpression ParseVariableExpression()
        {
            var name = MatchToken(TokenType.Identifier);

            return new VariableExpression(name);
        }

        private ArrayExpression ParseArrayExpression()
        {
            var openBracket = MatchToken(TokenType.OpenBracket);
            var items = ParseSeparatedExpressions(TokenType.CloseBracket);
            var closeBracket = MatchToken(TokenType.CloseBracket);

            return new ArrayExpression(openBracket, items, closeBracket);
        }

        private ImmutableArray<Expression> ParseSeparatedExpressions(TokenType endLimiter)
        {
            var expressions = ImmutableArray.CreateBuilder<Expression>();
            
            while (Current.Type != endLimiter && Current.Type != TokenType.Eof)
            {
                var expression = ParseExpression();
                expressions.Add(expression);
                
                if (Current.Type != TokenType.Comma)
                    break;

                MatchToken(TokenType.Comma);
            }

            return expressions.ToImmutable();
        }
        
        private SyntaxIndex ParseSyntaxIndex()
        {
            var openBracket = MatchToken(TokenType.OpenBracket);
            var index = ParseExpression();
            var closeBracket = MatchToken(TokenType.CloseBracket);

            return new SyntaxIndex(openBracket, index, closeBracket);
        }
    }
}