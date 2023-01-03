using System;
using System.Linq;
using Core;
using Ninject;
using Ninject.Extensions.Conventions;
using Array = System.Array;

namespace Repl;

public class Repl
{
    private readonly IConsolePrinter consolePrinter;
    private readonly ICommand[] commands;

    public Repl(IConsolePrinter consolePrinter, ICommand[] commands)
    {
        this.consolePrinter = consolePrinter;
        this.commands = commands;
    }
        
    public void Run()
    {
        var biteCompiler = BiteCompiler.Create();
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
                HandleSubmission(biteCompiler, interpreter, line);
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
            consolePrinter.PrintError($"Unknown command '{name}'");
            return;
        }

        var args = nameAndArgs.Length == 2
            ? nameAndArgs[1].Split()
            : Array.Empty<string>();
        
        command.Execute(args);
    }

    private void HandleSubmission(BiteCompiler compiler, Interpreter interpreter, string text)
    {
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
    
    public static Repl Create()
    {
        var container = new StandardKernel();

        container.Bind<IConsolePrinter>().To<ConsolePrinter>().InSingletonScope();
            
        container.Bind(conf => conf
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<ICommand>()
            .BindAllInterfaces());
            
        return container.Get<Repl>();
    }
}