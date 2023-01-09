using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Execution.Operations;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Syntax
{
    public class SyntaxParser : ISyntaxParser
    {
        private readonly Dictionary<TokenType, OperationPrecedence> binaryOperatorPrecedences;
        private readonly Dictionary<TokenType, OperationPrecedence> unaryOperatorPrecedences;
        private readonly Dictionary<TokenType, TokenType> compoundAssignmentOperators;

        private ImmutableArray<SyntaxToken> tokens;
        private DiagnosticBag diagnosticBag;
        private int position;
        private SourceText sourceText;

        public SyntaxParser(BinaryOperation[] binaryOperations, UnaryOperation[] unaryOperations)
        {
            binaryOperatorPrecedences = binaryOperations
                .ToDictionary(operation => operation.Operator, operation => operation.Precedence);

            compoundAssignmentOperators = binaryOperations
                .Where(operation => operation.CompoundAssignmentOperator is not null)
                .ToDictionary(operation => operation.CompoundAssignmentOperator.Value, operation => operation.Operator);
            
            unaryOperatorPrecedences = unaryOperations
                .ToDictionary(operation => operation.Operator, operation => operation.Precedence);
        }
        
        private SyntaxToken Current => Peek(0);
        
        private SyntaxToken Lookahead => Peek(1);

        private bool IsCurrentIdentifierWithEquals =>
            Current.Type == TokenType.Identifier && Lookahead.Type == TokenType.Equals;
        
        public TranslationState<SyntaxTree> Parse(SourceText text, IEnumerable<SyntaxToken> syntaxTokens)
        {
            position = 0;
            diagnosticBag = new DiagnosticBag();
            sourceText = text;
            tokens = syntaxTokens
                .Where(token => token.Type != TokenType.Space && token.Type != TokenType.LineBreak)
                .ToImmutableArray();
            
            var members = ImmutableArray.CreateBuilder<SyntaxNode>();

            try
            {
                while (Current.Type != TokenType.Eof)
                {
                    var startToken = Current;
                
                    var member = ParseStatement();
                    members.Add(member);

                    if (Current == startToken)
                        NextToken();
                }
            }
            catch (InvalidSyntaxException exception)
            {
                diagnosticBag.AddError(exception.Location, exception.Message);
            }

            var syntaxTree = new SyntaxTree(members.ToImmutable());
            return new TranslationState<SyntaxTree>(syntaxTree, diagnosticBag);
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

            throw new InvalidSyntaxException(Current.Location, expected);
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
                sourceText,
                keyword, 
                identifier, 
                openParenthesis, 
                positionParameters, 
                defaultParameters,
                closeParenthesis, 
                block
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

        private ImmutableArray<DefaultParameter> ParseDefaultParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<DefaultParameter>();
            
            while (IsCurrentIdentifierWithEquals)
            {
                var name = NextToken();
                var equals = NextToken();
                var expression = ParseBinaryExpression();

                var parameter = new DefaultParameter(name, equals, expression);
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

            return new ReturnStatement(sourceText, keyword, expression, semicolon);
        }

        private BreakStatement ParseBreakStatement()
        {
            var keyword = MatchToken(TokenType.BreakKeyword);
            var semicolon = MatchToken(TokenType.Semicolon);

            return new BreakStatement(sourceText, keyword, semicolon);
        }
        
        private ContinueStatement ParseContinueStatement()
        {
            var keyword = MatchToken(TokenType.ContinueKeyword);
            var semicolon = MatchToken(TokenType.Semicolon);

            return new ContinueStatement(sourceText, keyword, semicolon);
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
                sourceText,
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
            var openParenthesis = MatchToken(TokenType.OpenParenthesis);
            var condition = ParseExpression();
            var closeParenthesis = MatchToken(TokenType.CloseParenthesis);
            var body = ParseStatement();

            return new WhileStatement(sourceText, keyword, openParenthesis, condition, closeParenthesis, body);
        }
        
        private IfStatement ParseIfStatement()
        {
            var keyword = MatchToken(TokenType.IfKeyword);
            var openParenthesis = MatchToken(TokenType.OpenParenthesis);
            var condition = ParseExpression();
            var closeParenthesis = MatchToken(TokenType.CloseParenthesis);
            var thenStatement = ParseStatement();
            var elseClause = Current.Type == TokenType.ElseKeyword
                ? ParseElseClause()
                : null;

            return new IfStatement(
                sourceText, keyword, openParenthesis, condition, closeParenthesis, thenStatement, elseClause
            );
        }

        private ElseClause ParseElseClause()
        {
            var keyword = MatchToken(TokenType.ElseKeyword);
            var elseStatement = ParseStatement();

            return new ElseClause(sourceText, keyword, elseStatement);
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

            return new BlockStatement(sourceText, openBrace, statements.ToImmutable(), closeBrace);
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var expression = ParseExpression();
            var semicolon = MatchToken(TokenType.Semicolon);

            return new ExpressionStatement(sourceText, expression, semicolon);
        }

        private Expression ParseExpression()
        {
            var expression = ParseBinaryExpression();

            if (compoundAssignmentOperators.ContainsKey(Current.Type))
                return ContinueWithCompoundAssignmentExpression(expression);

            return Current.Type switch
            {
                TokenType.Equals => ContinueWithAssignmentExpression(expression),
                TokenType.QuestionMark => ContinueWithTernaryExpression(expression),
                _ => ParseBinaryExpression(initializedLeft: expression)
            };
        }

        private Expression ContinueWithCompoundAssignmentExpression(Expression to)
        {
            return to switch
            {
                VariableExpression toVariable => ContinueWithCompoundVariableAssignmentExpression(toVariable),
                IndexAccessExpression toIndex => ContinueWithCompoundIndexAssignmentExpression(toIndex),
                _ => throw new InvalidSyntaxException(to.Location, "Cannot assign to the expression")
            };
        }

        private Expression ContinueWithAssignmentExpression(Expression to)
        {
            return to switch
            {
                VariableExpression toVariable => ContinueWithVariableAssignmentExpression(toVariable),
                IndexAccessExpression toIndex => ContinueWithIndexAssignmentExpression(toIndex),
                _ => throw new InvalidSyntaxException(to.Location, "Cannot assign to the expression")
            };
        }

        private TernaryExpression ContinueWithTernaryExpression(Expression condition)
        {
            var questionMark = MatchToken(TokenType.QuestionMark);
            var thenExpression = ParseExpression();
            var colon = MatchToken(TokenType.Colon);
            var elseExpression = ParseExpression();

            return new TernaryExpression(sourceText, condition, questionMark, thenExpression, colon, elseExpression);
        }

        private IndexAssignmentExpression ContinueWithIndexAssignmentExpression(IndexAccessExpression indexAccessExpression)
        {
            var equals = MatchToken(TokenType.Equals);
            var value = ParseExpression();

            return new IndexAssignmentExpression(
                sourceText, indexAccessExpression.IndexedExpression, indexAccessExpression.Index, equals, value
            );
        }

        private IndexAssignmentExpression ContinueWithCompoundIndexAssignmentExpression(IndexAccessExpression indexAccessExpression)
        {
            var (compoundOperator, expression) = ContinueCompoundAssignmentExpression(indexAccessExpression);
            
            return new IndexAssignmentExpression(
                sourceText, 
                indexAccessExpression.IndexedExpression, 
                indexAccessExpression.Index, 
                compoundOperator,
                expression
            );
        }
        
        private VariableAssignmentExpression ContinueWithCompoundVariableAssignmentExpression(VariableExpression variableExpression)
        {
            var (compoundOperator, expression) = ContinueCompoundAssignmentExpression(variableExpression);

            return new VariableAssignmentExpression(
                sourceText, variableExpression.Token, compoundOperator, expression
            );
        }

        private (SyntaxToken compoundOperator, BinaryExpression expression) ContinueCompoundAssignmentExpression(Expression leftExpression)
        {
            var compoundOperator = NextToken();
            var singleOperatorType = compoundAssignmentOperators[compoundOperator.Type];

            var singleOperator = new SyntaxToken(singleOperatorType, compoundOperator.Text, compoundOperator.Location);
            var rightExpression = ParseExpression();

            return (compoundOperator, new BinaryExpression(sourceText, leftExpression, singleOperator, rightExpression));
        }
        
        private VariableAssignmentExpression ContinueWithVariableAssignmentExpression(VariableExpression variable)
        {
            var equals = MatchToken(TokenType.Equals);
            var expression = ParseExpression();

            return new VariableAssignmentExpression(sourceText, variable.Token, equals, expression);
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

                left = new BinaryExpression(sourceText, left, operatorToken, right);
            }

            return left;
        }

        private UnaryExpression ParseUnaryExpression()
        {
            var op = NextToken();
            var expression = ParseBinaryExpression(previousPrecedence: unaryOperatorPrecedences[op.Type]);

            return new UnaryExpression(sourceText, op, expression);
        }

        private Expression ParsePrimaryExpression()
        {
            Expression primaryExpression = Current.Type switch
            {
                TokenType.OpenParenthesis => ParseParenthesizedExpression(),
                TokenType.OpenBracket => ParseArrayExpression(),
                TokenType.TrueKeyword or TokenType.FalseKeyword => ParseBooleanExpression(),
                TokenType.NullKeyword => ParseNullExpression(),
                TokenType.Number => ParseNumberExpression(),
                TokenType.String => ParseStringExpression(),
                TokenType.Identifier => ParseVariableExpression(),
                _ => throw new InvalidSyntaxException(Current.Location)
            };
            
            while (Current.Type != TokenType.Eof)
            {
                var continuedExpression = Current.Type switch
                {
                    TokenType.OpenBracket => ContinueWithIndexAccessExpression(primaryExpression),
                    TokenType.OpenParenthesis => ContinueWithCallExpression(primaryExpression),
                    _ => null
                };

                if (continuedExpression is null)
                    break;

                primaryExpression = continuedExpression;
            }

            return primaryExpression;
        }

        private Expression ContinueWithIndexAccessExpression(Expression parent)
        {
            var index = ParseSyntaxIndex();

            return new IndexAccessExpression(sourceText, parent, index);
        }
        
        private CallExpression ContinueWithCallExpression(Expression expression)
        {
            var openParenthesis = MatchToken(TokenType.OpenParenthesis);
            var arguments = ParseSeparatedExpressions(TokenType.CloseParenthesis);
            var closeParenthesis = MatchToken(TokenType.CloseParenthesis);

            return new CallExpression(sourceText, expression, openParenthesis, arguments, closeParenthesis);
        }

        private ParenthesizedExpression ParseParenthesizedExpression()
        {
            var open = MatchToken(TokenType.OpenParenthesis);
            var expression = ParseExpression();
            var close = MatchToken(TokenType.CloseParenthesis);

            return new ParenthesizedExpression(sourceText, open, expression, close);
        }

        private NumberExpression ParseNumberExpression()
        {
            var token = MatchToken(TokenType.Number);
            
            return new NumberExpression(sourceText, token);
        }

        private BooleanExpression ParseBooleanExpression()
        {
            var token = Current.Type == TokenType.TrueKeyword
                ? MatchToken(TokenType.TrueKeyword) 
                : MatchToken(TokenType.FalseKeyword);

            return new BooleanExpression(sourceText, token);
        }

        private StringExpression ParseStringExpression()
        {
            var token = MatchToken(TokenType.String);

            return new StringExpression(sourceText, token);
        }

        private NullExpression ParseNullExpression()
        {
            var token = MatchToken(TokenType.NullKeyword);

            return new NullExpression(sourceText, token);
        }

        private VariableExpression ParseVariableExpression()
        {
            var token = MatchToken(TokenType.Identifier);

            return new VariableExpression(sourceText, token);
        }

        private ArrayExpression ParseArrayExpression()
        {
            var openBracket = MatchToken(TokenType.OpenBracket);
            var items = ParseSeparatedExpressions(TokenType.CloseBracket);
            var closeBracket = MatchToken(TokenType.CloseBracket);

            return new ArrayExpression(sourceText, openBracket, items, closeBracket);
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
        
        private Index ParseSyntaxIndex()
        {
            var openBracket = MatchToken(TokenType.OpenBracket);
            var index = ParseExpression();
            var closeBracket = MatchToken(TokenType.CloseBracket);

            return new Index(sourceText, openBracket, index, closeBracket);
        }
    }
}