namespace Repl.MetaCommands;

public class HelpCommand : IMetaCommand
{
    private readonly IPrinter printer;

    public HelpCommand(IPrinter printer)
    {
        this.printer = printer;
    }
    
    public string Name => "help";

    public string Description => "Show this message";

    public void Execute(string[] args)
    {
        printer.PrintHelp();
    }
}