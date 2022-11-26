using System.IO;

namespace Interpreter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var interpreter = new Core.Interpreter();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "main.txt");

            using var streamReader = File.OpenText(path);
            var text = streamReader.ReadToEnd();
            interpreter.Interpret(text);
        }
    }
}