using System;
using System.Globalization;
using Interpreter.Core;

namespace Repl
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            var core = new Interpreter.Core.Interpreter();
            
            while (true)
            {
                Console.Write(">> ");
                core.Interpret(Console.ReadLine());
            }
        }
    }
}