using System;
using System.Collections.Generic;
using System.Linq;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST;

namespace Translator.Core.Syntax
{
    public class SyntaxParser
    {
        private IReadOnlyList<SyntaxToken> tokens;
        private int position;

        private SyntaxToken Current => Peek(0);
        
        public SyntaxNode Parse(IReadOnlyList<SyntaxToken> tokens)
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

            return new SyntaxToken(expected, null);
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
                
                default:
                    return ParseNumberExpression();
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
            var number = MatchToken(TokenTypes.Number);
            
            return new NumberExpression(number);
        }
    }
}