using System;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax
{
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException(Location location, TokenType expected) : base($"Expected '{expected}'")
        {
            Location = location;
        }

        public InvalidSyntaxException(Location location) : base("Invalid syntax")
        {
            Location = location;
        }
        
        public Location Location { get; }
    }
}