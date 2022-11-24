using Translator.Core.Lexing;

namespace Translator.Core.Syntax
{
    public static class TokenTypesExtensions
    {
        public static int? GetBinaryOperatorPrecedence(this TokenTypes type)
        {
            switch (type)
            {
                case TokenTypes.Star:
                case TokenTypes.Slash:
                    return 100;

                case TokenTypes.Plus:
                case TokenTypes.Minus:
                    return 50;
                
                case TokenTypes.LeftArrow:
                case TokenTypes.RightArrow:
                    return 20;
                
                case TokenTypes.DoubleAmpersand:
                    return 10;

                case TokenTypes.DoublePipe:
                    return 5;

                default:
                    return null;
            }
        }
    }
}