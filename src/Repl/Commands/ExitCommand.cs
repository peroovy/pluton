using System;

namespace Repl.Commands;

public class ExitCommand : ICommand
{
    public string Name => "exit";

    public string Description => "Exit the REPL";
    
    public void Execute(string[] args)
    {
        Environment.Exit(0);
    }
}