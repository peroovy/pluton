using System;
using System.Linq;
using Core;
using Core.Execution.Objects;
using Array = System.Array;

namespace Repl;

public class Repl
{
    private readonly IErrorPrinter errorPrinter;
    private readonly IDiagnosticPrinter diagnosticPrinter;
    private readonly ICommand[] commands;

    public Repl(IErrorPrinter errorPrinter, IDiagnosticPrinter diagnosticPrinter, ICommand[] commands)
    {
        this.errorPrinter = errorPrinter;
        this.diagnosticPrinter = diagnosticPrinter;
        this.commands = commands;
    }
        
    public void Run()
    {
        var interpreter = Interpreter.Create();
            
        while (true)
        {
            Console.Write(">> ");

            var line = Console.ReadLine();

            if (line.StartsWith("#"))
            {
                HandleCommand(line);
            }
            else
            {
                HandleSubmission(interpreter, line);
            }
        }
    }

    private void HandleCommand(string line)
    {
        var nameAndArgs = line.Split(new[] { ' ' }, 2);
        var name = nameAndArgs[0].Substring(1);

        var command = commands.FirstOrDefault(command => command.Name == name);
        if (command is null)
        {
            errorPrinter.Print($"Unknown command '{name}'");
            return;
        }

        var args = nameAndArgs.Length == 2
            ? nameAndArgs[1].Split()
            : Array.Empty<string>();
        
        command.Execute(args);
    }

    private void HandleSubmission(Interpreter interpreter, string line)
    {
        try
        {
            var value = interpreter.Execute(line);

            if (value is Null) 
                return;
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(value.ToRepresentation());
            Console.ResetColor();
        }
        catch (InterpreterException)
        {
            diagnosticPrinter.Print(interpreter.DiagnosticBag);
        }
        finally
        {
            interpreter.DiagnosticBag.Reset();
        }
    }
}