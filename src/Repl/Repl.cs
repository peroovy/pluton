using System;
using System.Linq;
using Core;
using Ninject;
using Ninject.Extensions.Conventions;
using Repl.MetaCommands;
using Repl.KeyHandlers;
using Repl.Utils;

namespace Repl;

public class Repl
{
    private readonly Interpreter interpreter = Interpreter.Create();
    
    private readonly IMetaCommand[] commands;
    private readonly IKeyHandler[] keyHandlers;
    private readonly IPrinter printer;
    private readonly SubmissionHistory submissionHistory;

    private const string CommandFirstCharacter = "#";
    private const int BlankLineCountInEndSubmission = 3;

    private const ConsoleModifiers ForceEnterModifier = ConsoleModifiers.Shift;

    public Repl(IMetaCommand[] commands, IKeyHandler[] keyHandlers, IPrinter printer, SubmissionHistory submissionHistory)
    {
        this.commands = commands;
        this.keyHandlers = keyHandlers;
        this.printer = printer;
        this.submissionHistory = submissionHistory;
    }

    public void Run()
    {
        printer.PrintWelcome();
        
        while (true)
        {
            var text = EditSubmission();
            
            if (IsCommand(text)) HandleCommand(text);
            else HandleSubmission(text);
        }
    }

    private void HandleCommand(string text)
    {
        var nameAndArgs = text.Split(new[] { ' ' }, 2);
        var name = nameAndArgs[0].Substring(1);

        var command = commands.FirstOrDefault(command => command.Name == name);
        if (command is null)
        {
            printer.PrintError($"Unknown command '{name}'");
            return;
        }

        var args = nameAndArgs.Length == 2
            ? nameAndArgs[1].Split()
            : Array.Empty<string>();
        
        command.Execute(args);
    }
    
    private void HandleSubmission(string text)
    {
        var interpretation = interpreter.Execute(text);

        printer.PrintDiagnostic(interpretation.DiagnosticBag);
        
        if (!interpretation.HasErrors)
            printer.PrintResult(interpretation.Result, interpreter);
        
        printer.PrintBlankLine();
    }

    private string EditSubmission()
    {
        var document = new SubmissionDocument();
        var renderer = new SubmissionRenderer(document);

        while (true)
        {
            renderer.Render();
            
            var info = Console.ReadKey(true);

            if (info.Key == ConsoleKey.Enter)
            {
                if (info.Modifiers == ForceEnterModifier || IsCompleteSubmission(document))
                    return HandleSubmissionComplete(document, renderer);
                    
                document.AddNewLine(withHyphenation: info.Modifiers != ConsoleModifiers.Control);
            }

            HandleTyping(info, document);
        }
    }

    private string HandleSubmissionComplete(SubmissionDocument document, SubmissionRenderer renderer)
    {
        if (!document.IsEmpty)
        {
            renderer.Complete();
            submissionHistory.Add(document);
        }

        return document.ToString();
    }

    private void HandleTyping(ConsoleKeyInfo info, SubmissionDocument document)
    {
        var keyHandler = keyHandlers.FirstOrDefault(
            handler => handler.Key == info.Key && handler.Modifiers == info.Modifiers
        );

        if (keyHandler is not null)
        {
            keyHandler.Handle(info, document);
        }
        else if (info.KeyChar >= ' ')
        {
            document.Insert(info.KeyChar);
        }
    }

    private bool IsCompleteSubmission(SubmissionDocument document)
    {
        if (document is null || document.IsEmpty)
            return true;

        if (!document.IsEnd)
            return false;

        var text = document.ToString();
        
        if (IsCommand(text))
            return true;

        var lastLinesAreBlank = document
            .Reverse()
            .TakeWhile(SubmissionDocument.IsBlankLine)
            .Take(BlankLineCountInEndSubmission)
            .Count() == BlankLineCountInEndSubmission;
        
        if (lastLinesAreBlank)
            return true;

        var parsing = interpreter.Parse(text);
        return !parsing.HasErrors;
    }

    public static Repl Create()
    {
        var container = new StandardKernel();

        container.Bind<IPrinter>().To<ConsolePrinter>().InSingletonScope();
        container.Bind<SubmissionHistory>().ToSelf().InSingletonScope();
            
        container.Bind(conf => conf
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IMetaCommand>()
            .BindAllInterfaces());
        
        container.Bind(conf => conf
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IKeyHandler>()
            .BindAllInterfaces());
            
        return container.Get<Repl>();
    }
    
    private static bool IsCommand(string text) => text.StartsWith(CommandFirstCharacter);
}