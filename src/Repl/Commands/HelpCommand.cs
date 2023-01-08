using System;
using Repl.Utils;

namespace Repl.Commands;

public class HelpCommand : ICommand
{
    private readonly Lazy<ICommand[]> commands;

    public HelpCommand(Lazy<ICommand[]> commands)
    {
        this.commands = commands;
    }
    
    public string Name => "help";

    public string Description => "Show this message";

    public void Execute(string[] args)
    {
        Console.WriteLine(@"You can use:
Enter to execute a submission.
Arrows (↑ and ↓) to navigate within a multi-line submission.
Backspace to remove the character on the left.");
        Console.WriteLine();
        
        Console.WriteLine("Meta-commands available:");
        foreach (var command in commands.Value)
            Console.WriteLine($"#{command.Name} - {command.Description.Capitalize()}");
    }
}