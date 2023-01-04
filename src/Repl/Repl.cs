using System;
using System.Linq;
using Core;
using Ninject;
using Ninject.Extensions.Conventions;
using Repl.KeyHandlers;

namespace Repl;

public class Repl
{
    private readonly BiteCompiler compiler = BiteCompiler.Create();
    private readonly Interpreter interpreter = Interpreter.Create();
    
    private readonly ICommand[] commands;
    private readonly IKeyHandler[] keyHandlers;
    private readonly IPrinter printer;

    private int cursorTop;
    private int lastRenderedLineCount;

    private const string StartPrompt = ">> ";
    private const string ContinuePrompt = ".. ";
    private const int PromptLength = 3;
    
    private const string CommandStart = "#";

    public Repl(ICommand[] commands, IKeyHandler[] keyHandlers, IPrinter printer)
    {
        this.commands = commands;
        this.keyHandlers = keyHandlers;
        this.printer = printer;
    }

    public void Run()
    {
        while (true)
        {
            var text = EditSubmission();
            Console.WriteLine();
            
            if (IsCommand(text))
            {
                HandleCommand(text);
            }
            else
            {
                HandleSubmission(text);
            }
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
        var compilation = compiler.Compile(text);
        if (compilation.HasErrors)
        {
            printer.PrintDiagnostic(compilation.Diagnostic);
            return;
        }

        var interpretation = interpreter.Run(compilation.Result);
        if (interpretation.HasErrors)
        {
            printer.PrintDiagnostic(interpretation.Diagnostic);
            return;
        }

        printer.PrintInterpretationResult(interpretation.Result);
    }

    private string EditSubmission()
    {
        cursorTop = Console.CursorTop;
        
        var submissionDocument = new SubmissionDocument();
        submissionDocument.OnChanged += Render;

        submissionDocument.InsertEmptyLine();
        UpdateCursor(submissionDocument);
        
        while (true)
        {
            var info = Console.ReadKey(true);

            if (info.Key == ConsoleKey.Enter)
            {
                var text = string.Join(Environment.NewLine, submissionDocument);
                if (IsCommand(text) || IsSubmissionComplete(text))
                    return text;
                    
                submissionDocument.InsertEmptyLine();
            }

            var keyHandler = keyHandlers.FirstOrDefault(handler => handler.Key == info.Key);
            if (keyHandler is not null)
            {
                keyHandler.Handle(info, submissionDocument);
            }
            else if (info.KeyChar >= ' ')
            {
                submissionDocument.Insert(info.KeyChar);
            }
            
            UpdateCursor(submissionDocument);
        }
    }
    
    private bool IsSubmissionComplete(string text)
    {
        if (string.IsNullOrEmpty(text))
            return true;

        var lastTwoLinesAreBlank = text
            .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
            .Reverse()
            .TakeWhile(string.IsNullOrEmpty)
            .Take(2)
            .Count() == 2;
        
        if (lastTwoLinesAreBlank)
            return true;

        var compilation = compiler.Compile(text);

        return !compilation.HasErrors;
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
        Console.SetCursorPosition(PromptLength + submissionDocument.CharacterLeftIndex + 1,
            cursorTop + submissionDocument.LineIndex);
    }

    public static Repl Create()
    {
        var container = new StandardKernel();

        container.Bind<IPrinter>().To<Printer>().InSingletonScope();
            
        container.Bind(conf => conf
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<ICommand>()
            .BindAllInterfaces());
        
        container.Bind(conf => conf
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IKeyHandler>()
            .BindAllInterfaces());
            
        return container.Get<Repl>();
    }
    
    private static bool IsCommand(string text) => text.StartsWith(CommandStart);
}