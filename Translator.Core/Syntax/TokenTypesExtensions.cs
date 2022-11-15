using Translator.Core.Lexing;

namespace Translator.Core.Syntax
{
    public static class TokenTypesExtensions
    {
        public static int? GetBinaryOperatorPrecedence(this TokenTypes type)
        {
            switch (type)
            {
                case TokenTypes.Plus:
                case TokenTypes.Minus:
                    return 1;

                case TokenTypes.Star:
                case TokenTypes.Slash:
                    return 2;

                default:
                    return null;
            }
        }
    }
}