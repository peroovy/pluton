using System;
using System.IO;
using Core;

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
        
        var interpreter = Interpreter.Create();

        try
        {
            var absolutePath = Path.GetFullPath(args[0]);
            var code = File.ReadAllText(absolutePath);

            interpreter.Execute(code);
        }
        catch (ArgumentException)
        {
            errorPrinter.Print("Bad path");
        }
        catch (FileNotFoundException)
        {
            errorPrinter.Print("Not found file");
        }
        catch (InterpreterException)
        {
            diagnosticPrinter.Print(interpreter.DiagnosticBag);
        }
    }
}