using System;
using System.IO;
using Core;

namespace Repl.Commands;

public class FileCommand : ICommand
{
    private readonly IPrinter printer;

    public FileCommand(IPrinter printer)
    {
        this.printer = printer;
    }
    
    public string Name => "file";
    
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            printer.PrintError("Expected path to file");
            return;
        }

        try
        {
            var absolutePath = Path.GetFullPath(args[0]);
            var text = File.ReadAllText(absolutePath);
            
            var interpreter = Interpreter.Create();
            var interpretation = interpreter.Execute(text);
            printer.PrintDiagnostic(interpretation.Diagnostic);
        }
        catch (ArgumentException)
        {
            printer.PrintError("Bad path");
        }
        catch (FileNotFoundException)
        {
            printer.PrintError("Not found file");
        }
    }
}