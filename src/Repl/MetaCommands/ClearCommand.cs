using System;

namespace Repl.MetaCommands;

public class ClearCommand : IMetaCommand
{
    public string Name => "cls";

    public string Description => "Clear the console";

    public void Execute(string[] args) => Console.Clear();
}