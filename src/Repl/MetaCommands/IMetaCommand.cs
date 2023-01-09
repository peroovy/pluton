namespace Repl.MetaCommands;

public interface IMetaCommand
{
    string Name { get; }
    
    string Description { get; }

    void Execute(string[] args);
}