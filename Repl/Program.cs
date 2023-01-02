using System;
using System.Globalization;
using Core;

namespace Repl
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            var core = Interpreter.Create();
            
            while (true)
            {
                Console.Write("\n>> ");
                core.Interpret(Console.ReadLine());
            }
        }
    }
}