using System;
using System.Globalization;
using Interpreter.Core;
using Ninject;
using Interpreter.Core.Execution;
using Interpreter.Core.Lexing;
using Interpreter.Core.Logging;
using Interpreter.Core.Logging.Handlers;
using Interpreter.Core.Syntax;
using Interpreter.Core.Text;

namespace Repl
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            
            var core = Core.Configure();
            
            var logger = core.Get<ILogger>();
            var handler = core.Get<ILogHandler>();

            var textParser = core.Get<ITextParser>();
            var lexer = core.Get<ILexer>();
            var syntaxParser = core.Get<ISyntaxParser>();
            var evaluator = core.Get<IExecutor>();

            while (true)
            {
                Console.Write(">> ");
                var lines = textParser.ParseLines(Console.ReadLine());
                var tokens = lexer.Tokenize(lines);
                var syntaxTree = syntaxParser.Parse(tokens);

                if (logger.IsEmpty)
                    syntaxTree.Accept(evaluator);
                
                handler.Handle(logger);
                logger.Reset();
            }
        }
    }
}