using System;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Execution
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Location location, string message) : base(message)
        {
            Location = location;
        }
        
        public Location Location { get; }
    }
}