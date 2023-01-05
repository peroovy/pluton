using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Diagnostic;

namespace Core.Syntax
{
    public class SyntaxParser : ISyntaxParser
    {
        private ImmutableArray<SyntaxToken> tokens;
        private int position;

        private readonly IDiagnosticBag diagnosticBag;

        private SyntaxToken Current => Peek(0);
        
        public SyntaxParser(IDiagnosticBag diagnosticBag)
        {
            this.diagnosticBag = diagnosticBag;
        }

        public SyntaxTree Parse(ImmutableArray<SyntaxToken> syntaxTokens)
        {
            position = 0;
            tokens = syntaxTokens
                .Where(token => token.Type is not (TokenTypes.Space or TokenTypes.NewLine))
                .ToImmutableArray();
            
            var members = ImmutableArray.CreateBuilder<SyntaxNode>();
            while (Current.Type != TokenTypes.Eof)
            {
                var startToken = Current;
                
                var member = ParseStatement();
                members.Add(member);

                if (Current == startToken)
                    NextToken();
            }

            return new SyntaxTree(members.ToImmutable());
        }
        
        private SyntaxToken Peek(int offset)
        {
            var index = position + offset;
            
            return index >= tokens.Length ? tokens.Last() : tokens[index];
        }

        private SyntaxToken NextToken()
        {
            var current = Current;

            position++;

            return current;
        }
        
        private SyntaxToken MatchToken(TokenTypes expected)
        {
            if (Current.Type == expected)
                return NextToken();

            diagnosticBag.AddError(Current.Location, $"Expected '{expected}'");
            
            return new SyntaxToken(expected, null, Current.Location);
        }

        private Statement ParseStatement()
        {
            switch (Current.Type)
            {
                case TokenTypes.OpenBrace:
                    return ParseBlockStatement();
                
                case TokenTypes.IfKeyword:
                    return ParseIfStatement();

                case TokenTypes.WhileKeyword:
                    return ParseWhileStatement();
                
                case TokenTypes.ForKeyword:
                    return ParseForStatement();
                
                case TokenTypes.DefKeyword:
                    return ParseFunctionDeclarationStatement();
                
                case TokenTypes.ReturnKeyword:
                    return ParseReturnStatement();
                
                case TokenTypes.BreakKeyword:
                    return ParseBreakStatement();
                
                case TokenTypes.ContinueKeyword:
                    return ParseContinueStatement();
                
                default:
                    return ParseExpressionStatement();
            }
        }

        private FunctionDeclarationStatement ParseFunctionDeclarationStatement()
        {
            var keyword = MatchToken(TokenTypes.DefKeyword);
            var name = MatchToken(TokenTypes.Identifier);
            var openParenthesis = MatchToken(TokenTypes.OpenParenthesis);
            var positionParameters = ParsePositionParameters();
            var defaultParameters = ParseDefaultParameters();
            var closeParenthesis = MatchToken(TokenTypes.CloseParenthesis);
            var body = ParseBlockStatement();

            return new FunctionDeclarationStatement(
                keyword, name, openParenthesis, positionParameters, defaultParameters,
                closeParenthesis, body
            );
        }

        private ImmutableArray<SyntaxToken> ParsePositionParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<SyntaxToken>();
            
            while (Current.Type is not TokenTypes.CloseParenthesis or TokenTypes.Eof
                   && !(Peek(0).Type == TokenTypes.Identifier && Peek(1).Type == TokenTypes.Equals))
            {
                var parameter = MatchToken(TokenTypes.Identifier);
                parameters.Add(parameter);
                
                if (Current.Type != TokenTypes.Comma)
                    break;

                MatchToken(TokenTypes.Comma);
            }

            return parameters.ToImmutable();
        }

        private ImmutableArray<SyntaxDefaultParameter> ParseDefaultParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<SyntaxDefaultParameter>();
            
            while (Current.Type is not TokenTypes.CloseParenthesis or TokenTypes.Eof
                   && Peek(0).Type == TokenTypes.Identifier && Peek(1).Type == TokenTypes.Equals)
            {
                var name = MatchToken(TokenTypes.Identifier);
                var equals = MatchToken(TokenTypes.Equals);
                var expression = ParseBinaryExpression();

                var parameter = new SyntaxDefaultParameter(name, equals, expression);
                parameters.Add(parameter);
                
                if (Current.Type != TokenTypes.Comma)
                    break;

                MatchToken(TokenTypes.Comma);
            }

            return parameters.ToImmutable();
        }

        private ReturnStatement ParseReturnStatement()
        {
            var keyword = MatchToken(TokenTypes.ReturnKeyword);
            var expression = Current.Type == TokenTypes.Semicolon ? null : ParseExpression();
            var semicolon = MatchToken(TokenTypes.Semicolon);

            return new ReturnStatement(keyword, expression, semicolon);
        }

        private BreakStatement ParseBreakStatement()
        {
            var keyword = MatchToken(TokenTypes.BreakKeyword);
            var semicolon = MatchToken(TokenTypes.Semicolon);

            return new BreakStatement(keyword, semicolon);
        }
        
        private ContinueStatement ParseContinueStatement()
        {
            var keyword = MatchToken(TokenTypes.ContinueKeyword);
            var semicolon = MatchToken(TokenTypes.Semicolon);

            return new ContinueStatement(keyword, semicolon);
        }

        private ForStatement ParseForStatement()
        {
            var keyword = MatchToken(TokenTypes.ForKeyword);
            var openParenthesis = MatchToken(TokenTypes.OpenParenthesis);
            var initializers = ParseSeparatedExpressions(TokenTypes.Semicolon);
            var firstSemicolon = MatchToken(TokenTypes.Semicolon);
            var condition = ParseExpression();
            var secondSemicolon = MatchToken(TokenTypes.Semicolon);
            var iterators = ParseSeparatedExpressions(TokenTypes.CloseParenthesis);
            var closeParenthesis = MatchToken(TokenTypes.CloseParenthesis);
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
            var keyword = MatchToken(TokenTypes.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();

            return new WhileStatement(keyword, condition, body);
        }
        
        private IfStatement ParseIfStatement()
        {
            var keyword = MatchToken(TokenTypes.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseClause = ParseElseClause();

            return new IfStatement(keyword, condition, statement, elseClause);
        }

        private ElseClause ParseElseClause()
        {
            if (Current.Type != TokenTypes.ElseKeyword)
                return null;

            var keyword = MatchToken(TokenTypes.ElseKeyword);
            var statement = ParseStatement();

            return new ElseClause(keyword, statement);
        }

        private BlockStatement ParseBlockStatement()
        {
            var openBrace = MatchToken(TokenTypes.OpenBrace);

            var statements = ImmutableArray.CreateBuilder<Statement>();
            while (Current.Type != TokenTypes.CloseBrace && Current.Type != TokenTypes.Eof)
            {
                var startToken = Current;
                
                var statement = ParseStatement();
                statements.Add(statement);

                if (Current == startToken)
                    NextToken();
            }

            var closeBrace = MatchToken(TokenTypes.CloseBrace);

            return new BlockStatement(openBrace, statements.ToImmutable(), closeBrace);
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var expression = ParseExpression();
            var semicolon = MatchToken(TokenTypes.Semicolon);

            return new ExpressionStatement(expression, semicolon);
        }

        private Expression ParseExpression()
        {
            var expression = ParseBinaryExpression();

            switch (Current.Type)
            {
                case TokenTypes.Equals:
                {
                    switch (expression)
                    {
                        case VariableExpression variableExpression:
                            return ParseVariableAssignmentExpression(variableExpression);
                        
                        case IndexAccessExpression indexAccessExpression:
                            return ParseIndexAssignmentExpression(indexAccessExpression);
                    }
                    break;
                }

                case TokenTypes.PlusEquals:
                case TokenTypes.MinusEquals:
                case TokenTypes.StarEquals:
                case TokenTypes.SlashEquals:
                {
                    switch (expression)
                    {
                        case VariableExpression variableExpression:
                            return ParseCompoundVariableAssignmentExpression(variableExpression);
                        
                        case IndexAccessExpression indexAccessExpression:
                            return ParseCompoundIndexAssignmentExpression(indexAccessExpression);
                    }
                    break;
                }
            }
            
            return ParseBinaryExpression(expression);
        }

        private IndexAssignmentExpression ParseIndexAssignmentExpression(IndexAccessExpression indexAccessExpression)
        {
            var equals = MatchToken(TokenTypes.Equals);
            var value = ParseExpression();

            return new IndexAssignmentExpression(
                indexAccessExpression.ParentExpression, indexAccessExpression.Index, equals, value
            );
        }

        private IndexAssignmentExpression ParseCompoundIndexAssignmentExpression(
            IndexAccessExpression indexAccessExpression)
        {
            var (compoundOperator, expression) = ParseCompoundAssignmentExpression(indexAccessExpression);
            
            return new IndexAssignmentExpression(
                indexAccessExpression.ParentExpression, indexAccessExpression.Index, compoundOperator, expression
            );
        }

        private VariableAssignmentExpression ParseCompoundVariableAssignmentExpression(VariableExpression variableExpression)
        {
            var (compoundOperator, expression) = ParseCompoundAssignmentExpression(variableExpression);
            
            return new VariableAssignmentExpression(variableExpression.Name, compoundOperator, expression);
        }

        private (SyntaxToken compoundOperator, BinaryExpression expression) ParseCompoundAssignmentExpression(
            Expression leftExpression)
        {
            var compoundOperator = NextToken();
            var singleOperatorType = compoundOperator.Type.TryConvertCompoundOperatorToSingle();
            if (singleOperatorType is null)
                throw new ArgumentException($"Unknown compound operator '{compoundOperator.Type}'");

            var singleOperator = new SyntaxToken(
                singleOperatorType.Value, compoundOperator.Text, compoundOperator.Location
            );
            var rightExpression = ParseExpression();

            return (compoundOperator, new BinaryExpression(leftExpression, singleOperator, rightExpression));
        }

        private VariableAssignmentExpression ParseVariableAssignmentExpression(VariableExpression variable)
        {
            var equals = MatchToken(TokenTypes.Equals);
            var expression = ParseExpression();

            return new VariableAssignmentExpression(variable.Name, equals, expression);
        }

        private Expression ParseBinaryExpression(Expression leftInit = null, int parentPrecedence = 0)
        {
            var unaryPrecedence = Current.Type.TryGetUnaryPrecedence();
            var left = leftInit ?? (unaryPrecedence >= parentPrecedence
                ? ParseUnaryExpression(unaryPrecedence.Value) 
                : ParsePrimaryExpression());

            while (true)
            {
                var precedence = Current.Type.TryGetBinaryOperatorPrecedence();
                if (precedence is null || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(parentPrecedence: precedence.Value);

                left = new BinaryExpression(left, operatorToken, right);
            }

            return left;
        }

        private UnaryExpression ParseUnaryExpression(int unaryPrecedence)
        {
            var op = NextToken();
            var expression = ParseBinaryExpression(parentPrecedence: unaryPrecedence);

            return new UnaryExpression(op, expression);
        }

        private Expression ParsePrimaryExpression()
        {
            Expression primary;
            
            switch (Current.Type)
            {
                case TokenTypes.OpenParenthesis:
                    primary = ParseParenthesizedExpression(); 
                    break;
                
                case TokenTypes.OpenBracket:
                    primary = ParseArrayExpression(); 
                    break;
                
                case TokenTypes.TrueKeyword:
                case TokenTypes.FalseKeyword:
                    primary = ParseBooleanExpression();
                    break;
                
                case TokenTypes.NullKeyword:
                    primary = ParseNullExpression();
                    break;
                
                case TokenTypes.Number:
                    primary = ParseNumberExpression();
                    break;

                case TokenTypes.String:
                    primary = ParseStringExpression();
                    break;
                
                default:
                    primary = ParseVariableOrCallExpression();
                    break;
            }

            if (Current.Type == TokenTypes.OpenBracket)
                primary = ParseIndexAccessExpression(primary);

            return primary;
        }

        private Expression ParseIndexAccessExpression(Expression parent)
        {
            do
            {
                var index = ParseSyntaxIndex();

                parent = new IndexAccessExpression(parent, index);
                
            } while (Current.Type == TokenTypes.OpenBracket && Current.Type != TokenTypes.Eof);

            return parent;
        }

        private ParenthesizedExpression ParseParenthesizedExpression()
        {
            var open = MatchToken(TokenTypes.OpenParenthesis);
            var expression = ParseExpression();
            var close = MatchToken(TokenTypes.CloseParenthesis);

            return new ParenthesizedExpression(open, expression, close);
        }

        private NumberExpression ParseNumberExpression()
        {
            var numberToken = MatchToken(TokenTypes.Number);
            var value = Convert.ToDouble(numberToken.Text, CultureInfo.InvariantCulture);
            
            return new NumberExpression(value);
        }

        private BooleanExpression ParseBooleanExpression()
        {
            var value = Current.Type == TokenTypes.TrueKeyword;
            _ = value ? MatchToken(TokenTypes.TrueKeyword) : MatchToken(TokenTypes.FalseKeyword);

            return new BooleanExpression(value);
        }

        private StringExpression ParseStringExpression()
        {
            var token = MatchToken(TokenTypes.String);

            return new StringExpression(token.Text);
        }

        private NullExpression ParseNullExpression()
        {
            MatchToken(TokenTypes.NullKeyword);

            return new NullExpression();
        }

        private Expression ParseVariableOrCallExpression()
        {
            if (Peek(0).Type == TokenTypes.Identifier && Peek(1).Type == TokenTypes.OpenParenthesis)
                return ParseCallExpression();

            return ParseVariableExpression();
        }

        private FunctionCallExpression ParseCallExpression()
        {
            var name = MatchToken(TokenTypes.Identifier);
            var openParenthesis = MatchToken(TokenTypes.OpenParenthesis);
            var arguments = ParseSeparatedExpressions(TokenTypes.CloseParenthesis);
            var closeParenthesis = MatchToken(TokenTypes.CloseParenthesis);

            return new FunctionCallExpression(name, openParenthesis, arguments, closeParenthesis);
        }

        private VariableExpression ParseVariableExpression()
        {
            var name = MatchToken(TokenTypes.Identifier);

            return new VariableExpression(name);
        }

        private ListExpression ParseArrayExpression()
        {
            var openBracket = MatchToken(TokenTypes.OpenBracket);
            var items = ParseSeparatedExpressions(TokenTypes.CloseBracket);
            var closeBracket = MatchToken(TokenTypes.CloseBracket);

            return new ListExpression(openBracket, items, closeBracket);
        }

        private ImmutableArray<Expression> ParseSeparatedExpressions(TokenTypes endLimiter)
        {
            var expressions = ImmutableArray.CreateBuilder<Expression>();
            
            while (Current.Type != endLimiter && Current.Type != TokenTypes.Eof)
            {
                var expression = ParseExpression();
                expressions.Add(expression);
                
                if (Current.Type != TokenTypes.Comma)
                    break;

                MatchToken(TokenTypes.Comma);
            }

            return expressions.ToImmutable();
        }
        
        private SyntaxIndex ParseSyntaxIndex()
        {
            var openBracket = MatchToken(TokenTypes.OpenBracket);
            var index = ParseExpression();
            var closeBracket = MatchToken(TokenTypes.CloseBracket);

            return new SyntaxIndex(openBracket, index, closeBracket);
        }
    }
}