using System;

namespace Translator.Core.Logging.Handlers
{
    public class ConsoleHandler : ILogHandler
    {
        private const int CodeIndent = 6;
        
        private const ConsoleColor ErrorColor = ConsoleColor.DarkRed;

        private const ConsoleColor CodeFontColor = ConsoleColor.White;
        private const ConsoleColor HighlightBackColor = ConsoleColor.DarkRed;
        
        public void Handle(ILogger logger)
        {
            foreach (var log in logger.Bucket)
            {
                PrintMessage(log);
                PrintCode(log);

                Console.ResetColor();
                Console.WriteLine();
            }
        }

        private static void PrintMessage(Log log)
        {
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine(log.Message);
        }

        private static void PrintCode(Log log)
        {
            var line = log.Location.Line.Value;
            var position = log.Location.Position;
            var length = log.LengthError;
            
            Console.Write(new string(' ', CodeIndent));

            Console.ForegroundColor = CodeFontColor;
            Console.Write(line.Substring(0, position));
            Console.ResetColor();

            Console.BackgroundColor = HighlightBackColor;
            Console.Write(line.Substring(position, length));
            Console.ResetColor();

            Console.ForegroundColor = CodeFontColor;
            Console.Write(line.Substring(position + length));
        }
    }
}