using System;
using Core;
using Core.Execution.Objects;

namespace Repl;

public class Repl
{
    private readonly IDiagnosticPrinter diagnosticPrinter;

    public Repl(IDiagnosticPrinter diagnosticPrinter)
    {
        this.diagnosticPrinter = diagnosticPrinter;
    }
        
    public void Run()
    {
        var interpreter = Interpreter.Create();
            
        while (true)
        {
            Console.Write(">> ");

            var code = Console.ReadLine();

            try
            {
                var value = interpreter.Execute(code);

                if (value is not Null)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(value.ToRepresentation());
                    Console.ResetColor();
                }
            }
            catch (InterpreterException)
            {
                diagnosticPrinter.Print(interpreter.DiagnosticBag);
            }
            finally
            {
                interpreter.Reset();
            }
        }
    }
}