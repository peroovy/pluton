using System;
using System.IO;
using Core;
using Core.Execution.Objects;

namespace Repl;

public class FileCommand : ICommand
{
    private readonly IErrorPrinter errorPrinter;
    private readonly IDiagnosticPrinter diagnosticPrinter;

    public FileCommand(IErrorPrinter errorPrinter, IDiagnosticPrinter diagnosticPrinter)
    {
        this.errorPrinter = errorPrinter;
        this.diagnosticPrinter = diagnosticPrinter;
    }
    
    public string Name => "file";
    
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            errorPrinter.Print("Expected path to file");
            return;
        }

        var compiler = BiteCompiler.Create();
        var interpreter = Interpreter.Create();

        try
        {
            var absolutePath = Path.GetFullPath(args[0]);
            var text = File.ReadAllText(absolutePath);

            var compilation = compiler.Compile(text);
            if (compilation.HasErrors)
            {
                diagnosticPrinter.Print(compilation.Diagnostic);
                return;
            }

            var interpretation = interpreter.Run(compilation.Result);
            if (interpretation.HasErrors)
            {
                diagnosticPrinter.Print(interpretation.Diagnostic);
                return;
            }

            var value = interpretation.Result;
            if (value is Null)
                return;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(value);
            Console.ResetColor();
        }
        catch (ArgumentException)
        {
            errorPrinter.Print("Bad path");
        }
        catch (FileNotFoundException)
        {
            errorPrinter.Print("Not found file");
        }
    }
}