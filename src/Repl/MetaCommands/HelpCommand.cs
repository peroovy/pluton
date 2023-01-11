using System;
using Repl.Utils;

namespace Repl.MetaCommands;

public class HelpCommand : IMetaCommand
{
    private readonly Lazy<IMetaCommand[]> commands;

    public HelpCommand(Lazy<IMetaCommand[]> commands)
    {
        this.commands = commands;
    }
    
    public string Name => "help";

    public string Description => "Show this message";

    public void Execute(string[] args)
    {
        Console.WriteLine(@"You can use:

Enter to execute a submission or go to the next line with hyphenation.
Shift+Enter to force start execution.
Ctrl+Enter to go to the next line.
Arrows (↑ and ↓) to navigate within a multi-line submission.
PageUp and PageDown to navigate through submission history.
Backspace to remove the character on the left.
Delete to remove the character on the right.
Ctrl+Backspace to delete the entered submission.");
        
        Console.WriteLine();
        Console.WriteLine();
        
        Console.WriteLine("Meta-commands available:");
        Console.WriteLine();
        foreach (var command in commands.Value)
            Console.WriteLine($"#{command.Name} - {command.Description.Capitalize()}");
        
        Console.WriteLine();
    }
}