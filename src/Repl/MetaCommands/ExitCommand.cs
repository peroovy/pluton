using System;

namespace Repl.MetaCommands;

public class ExitCommand : IMetaCommand
{
    public string Name => "exit";

    public string Description => "Exit the REPL";
    
    public void Execute(string[] args)
    {
        Environment.Exit(0);
    }
}