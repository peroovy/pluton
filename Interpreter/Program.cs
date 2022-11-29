using System.IO;
using Interpreter.Core;

namespace Interpreter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var core = InterpreterCore.Create();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "main.txt");

            using var streamReader = File.OpenText(path);
            var text = streamReader.ReadToEnd();
            core.Interpret(text);
        }
    }
}