using System;

namespace Repl;

public class ClearCommand : ICommand
{
    public string Name => "cls";

    public void Execute(string[] args) => Console.Clear();
}