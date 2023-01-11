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

    private int cursorTop;
    private int lastRenderedLineCount;

    private const string StartPrompt = ">> ";
    private const string ContinuePrompt = ".. ";
    private const int PromptLength = 3;
    
    private const string CommandFirstCharacter = "#";

    private const int BlankLineCountInEndSubmission = 3;

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
            printer.PrintResult(interpretation.Result);
        
        printer.PrintBlankLine();
    }

    private string EditSubmission()
    {
        cursorTop = Console.CursorTop;
        
        var submissionDocument = new SubmissionDocument();
        submissionDocument.OnChanged += Render;
        
        Render(submissionDocument);
        UpdateCursor(submissionDocument);
        
        while (true)
        {
            var info = Console.ReadKey(true);

            if (info.Key == ConsoleKey.Enter)
            {
                if (IsCompleteSubmission(submissionDocument))
                    return HandleSubmissionComplete(submissionDocument);
                    
                submissionDocument.AddNewLine();
            }

            HandleTyping(info, submissionDocument);
            UpdateCursor(submissionDocument);
        }
    }

    private string HandleSubmissionComplete(SubmissionDocument submissionDocument)
    {
        submissionDocument.OnChanged -= Render;

        if (!submissionDocument.IsEmpty)
        {
            Console.SetCursorPosition(0, cursorTop + submissionDocument.LineCount);
            submissionHistory.Add(submissionDocument);
        }

        return submissionDocument.ToString();
    }

    private void HandleTyping(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        var keyHandler = keyHandlers.FirstOrDefault(handler =>
        {
            var result = handler.Key == info.Key;

            if (handler.Modifiers != default)
                result = result && (handler.Modifiers & info.Modifiers) > 0;

            return result;
        });

        if (keyHandler is not null)
        {
            keyHandler.Handle(info, submissionDocument);
        }
        else if (info.KeyChar >= ' ')
        {
            submissionDocument.Insert(info.KeyChar);
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

    private void Render(SubmissionDocument submissionDocument)
    {
        Console.CursorVisible = false;
        var lineCount = 0;

        foreach (var line in submissionDocument)
        {
            Console.SetCursorPosition(0, cursorTop + lineCount);

            var prompt = lineCount == 0 ? StartPrompt : ContinuePrompt;
            var textLength = prompt.Length + line.Length;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(prompt);
            Console.ResetColor();

            Console.Write(line);
            Console.Write(new string(' ', Console.WindowWidth - textLength - 1));

            lineCount++;
        }
        
        var blankLinesCount = lastRenderedLineCount - lineCount;
        if (blankLinesCount > 0)
        {
            var blankLine = new string(' ', Console.WindowWidth);
            for (var i = 0; i < blankLinesCount; i++)
            {
                Console.SetCursorPosition(0, cursorTop + lineCount + i);
                Console.WriteLine(blankLine);
            }
        }
        lastRenderedLineCount = lineCount;
        
        Console.CursorVisible = true;
    }

    private void UpdateCursor(SubmissionDocument submissionDocument)
    {
        Console.SetCursorPosition(
            PromptLength + submissionDocument.CharacterIndex,
            cursorTop + submissionDocument.LineIndex
        );
    }

    public static Repl Create()
    {
        var container = new StandardKernel();

        container.Bind<IPrinter>().To<Printer>().InSingletonScope();
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