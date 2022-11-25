using System.Collections.Generic;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax
{
    public static class SyntaxOperatorsExtensions
    {
        private static readonly HashSet<TokenTypes> UnaryOperators = new()
        {
            TokenTypes.Plus,
            TokenTypes.Minus,
            TokenTypes.ExclamationMark
        };

        private static readonly Dictionary<TokenTypes, TokenTypes> CompoundOperatorsToSingle = new()
        {
            [TokenTypes.PlusEquals] = TokenTypes.Plus,
            [TokenTypes.MinusEquals] = TokenTypes.Minus,
            [TokenTypes.StarEquals] = TokenTypes.Star,
            [TokenTypes.SlashEquals] = TokenTypes.Slash
        };

        public static TokenTypes? TryConvertCompoundOperatorToSingle(this TokenTypes type)
        {
            if (!CompoundOperatorsToSingle.TryGetValue(type, out var single))
                return null;

            return single;
        }

        public static int? TryGetBinaryOperatorPrecedence(this TokenTypes type)
        {
            switch (type)
            {
                case TokenTypes.Star:
                case TokenTypes.Slash:
                case TokenTypes.Percent:
                    return 100;

                case TokenTypes.Plus:
                case TokenTypes.Minus:
                    return 50;
                
                case TokenTypes.LeftArrow:
                case TokenTypes.RightArrow:
                case TokenTypes.LeftArrowEquals:
                case TokenTypes.RightArrowEquals:
                case TokenTypes.DoubleEquals:
                case TokenTypes.ExclamationMarkEquals:
                    return 20;
                
                case TokenTypes.DoubleAmpersand:
                    return 10;

                case TokenTypes.DoublePipe:
                    return 5;

                default:
                    return null;
            }
        }

        public static int? TryGetUnaryPrecedence(this TokenTypes type) => UnaryOperators.Contains(type) ? 10000 : null;
    }
}