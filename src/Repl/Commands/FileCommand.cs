using System;
using System.IO;
using Core;

namespace Repl.Commands;

public class FileCommand : ICommand
{
    private readonly IConsolePrinter consolePrinter;

    public FileCommand(IConsolePrinter consolePrinter)
    {
        this.consolePrinter = consolePrinter;
    }
    
    public string Name => "file";
    
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            consolePrinter.PrintError("Expected path to file");
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
                consolePrinter.PrintDiagnostic(compilation.Diagnostic);
                return;
            }

            var interpretation = interpreter.Run(compilation.Result);
            if (interpretation.HasErrors)
            {
                consolePrinter.PrintDiagnostic(interpretation.Diagnostic);
                return;
            }
            
            consolePrinter.PrintInterpretationResult(interpretation.Result);
        }
        catch (ArgumentException)
        {
            consolePrinter.PrintError("Bad path");
        }
        catch (FileNotFoundException)
        {
            consolePrinter.PrintError("Not found file");
        }
    }
}