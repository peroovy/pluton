using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Translator.Core.Lexing;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax
{
    public class SyntaxParser : ISyntaxParser
    {
        private IReadOnlyList<SyntaxToken> tokens;
        private int position;

        private readonly ILogger logger;

        private SyntaxToken Current => Peek(0);
        
        public SyntaxParser(ILogger logger)
        {
            this.logger = logger;
        }

        public SyntaxNode Parse(ImmutableArray<SyntaxToken> syntaxTokens)
        {
            tokens = syntaxTokens;
            position = 0;

            return ParseStatement();
        }
        
        private SyntaxToken Peek(int offset)
        {
            var index = position + offset;
            
            return index >= tokens.Count ? tokens.Last() : tokens[index];
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

            logger.Error(Current.Location, Current.Length, $"Expected '{expected}' but was '{Current.Type}'");
            
            return new SyntaxToken(expected, null, Current.Location);
        }

        private Statement ParseStatement()
        {
            switch (Current.Type)
            {
                case TokenTypes.OpenBrace:
                    return ParseBlockStatement();
                
                default:
                    return ParseExpressionStatement();
            }
        }

        private BlockStatement ParseBlockStatement()
        {
            var openBrace = MatchToken(TokenTypes.OpenBrace);

            var statements = ImmutableArray.CreateBuilder<Statement>();
            while (Current.Type != TokenTypes.CloseBrace && Current.Type != TokenTypes.Eof)
            {
                var statement = ParseStatement();
                
                statements.Add(statement);
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
            if (Current.Type == TokenTypes.Identifier && Peek(1).Type == TokenTypes.Equals)
                return ParseAssignmentExpression();
            
            return ParseBinaryExpression();
        }

        private AssignmentExpression ParseAssignmentExpression()
        {
            var variable = MatchToken(TokenTypes.Identifier);
            var equals = MatchToken(TokenTypes.Equals);
            var expression = ParseExpression();

            return new AssignmentExpression(variable, equals, expression);
        }

        private Expression ParseBinaryExpression(int parentPrecedence = 0)
        {
            var left = ParsePrimaryExpression();

            while (true)
            {
                var precedence = Current.Type.GetBinaryOperatorPrecedence();
                if (precedence is null || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence.Value);

                left = new BinaryExpression(left, operatorToken, right);
            }

            return left;
        }

        private Expression ParsePrimaryExpression()
        {
            switch (Current.Type)
            {
                case TokenTypes.OpenParenthesis:
                    return ParseParenthesizedExpression();
                
                case TokenTypes.TrueKeyword:
                case TokenTypes.FalseKeyword:
                    return ParseBooleanExpression();
                
                case TokenTypes.Number:
                    return ParseNumberExpression();
                
                default:
                    return ParseVariableExpression();
            }
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
            var value = Convert.ToDouble(numberToken.Text);
            
            return new NumberExpression(value);
        }

        private BooleanExpression ParseBooleanExpression()
        {
            var value = Current.Type == TokenTypes.TrueKeyword;
            _ = value ? MatchToken(TokenTypes.TrueKeyword) : MatchToken(TokenTypes.FalseKeyword);

            return new BooleanExpression(value);
        }

        private VariableExpression ParseVariableExpression()
        {
            var name = MatchToken(TokenTypes.Identifier);

            return new VariableExpression(name);
        }
    }
}