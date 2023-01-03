using System;
using Core.Diagnostic;

namespace Repl
{
    public class DiagnosticPrinter : IDiagnosticPrinter
    {
        private const int CodeIndent = 6;
        
        private const ConsoleColor ErrorColor = ConsoleColor.DarkRed;

        private const ConsoleColor CodeFontColor = ConsoleColor.White;
        private const ConsoleColor HighlightBackColor = ConsoleColor.DarkRed;
        
        public void Print(IDiagnosticBag diagnosticBag)
        {
            foreach (var log in diagnosticBag)
            {
                PrintMessage(log);
                PrintCode(log);

                Console.ResetColor();
                Console.WriteLine();
            }
        }

        private static void PrintMessage(Log log)
        {
            var location = log.Location;
            var message = $"{log.Level}({location.Line.Number}, {location.Position + 1}): {log.Message}";
            
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine(message);
        }

        private static void PrintCode(Log log)
        {
            var location = log.Location;
            
            var line = location.Line.Value;
            var trimmedLine = line.TrimStart();
                
            var position = location.Position - (line.Length - trimmedLine.Length);
            
            Console.Write(new string(' ', CodeIndent));

            Console.ForegroundColor = CodeFontColor;
            Console.Write(trimmedLine.Substring(0, position));
            Console.ResetColor();

            Console.BackgroundColor = HighlightBackColor;
            Console.Write(trimmedLine.Substring(position, log.HighlightCodeLength));
            Console.ResetColor();

            Console.ForegroundColor = CodeFontColor;
            Console.Write(trimmedLine.Substring(position + log.HighlightCodeLength));
        }
    }
}