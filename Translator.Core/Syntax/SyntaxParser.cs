using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Translator.Core.Lexing;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;

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

        public SyntaxNode Parse(ImmutableArray<SyntaxToken> tokens)
        {
            this.tokens = tokens;
            position = 0;

            return ParseExpression();
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

        private Expression ParseExpression()
        {
            return ParseBinaryExpression();
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
                    throw new InvalidOperationException($"Unparsed token {Current.Type}");
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
            var value = Convert.ToDouble(numberToken.Value);
            
            return new NumberExpression(value);
        }

        private BooleanExpression ParseBooleanExpression()
        {
            var value = Current.Type == TokenTypes.TrueKeyword;
            var token = value ? MatchToken(TokenTypes.TrueKeyword) : MatchToken(TokenTypes.FalseKeyword);

            return new BooleanExpression(value);
        }
    }
}