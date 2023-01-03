using System;

namespace Repl.Commands;

public class ClearCommand : ICommand
{
    public string Name => "cls";

    public void Execute(string[] args) => Console.Clear();
}