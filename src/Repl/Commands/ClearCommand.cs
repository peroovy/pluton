using System;

namespace Repl.Commands;

public class ClearCommand : ICommand
{
    public string Name => "cls";

    public string Description => "Clear the console";

    public void Execute(string[] args) => Console.Clear();
}