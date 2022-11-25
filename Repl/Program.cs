using System;
using System.Globalization;
using Ninject;
using Translator.Core;
using Translator.Core.Execution;
using Translator.Core.Lexing;
using Translator.Core.Logging;
using Translator.Core.Logging.Handlers;
using Translator.Core.Syntax;
using Translator.Core.Text;

namespace Repl
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            
            var interpreter = Interpreter.Configure();
            
            var logger = interpreter.Get<ILogger>();
            var handler = interpreter.Get<ILogHandler>();

            var textParser = interpreter.Get<ITextParser>();
            var lexer = interpreter.Get<ILexer>();
            var syntaxParser = interpreter.Get<ISyntaxParser>();
            var evaluator = interpreter.Get<IExecutor>();

            while (true)
            {
                Console.Write(">> ");
                var lines = textParser.ParseLines(Console.ReadLine());
                var tokens = lexer.Tokenize(lines);
                var syntaxNode = syntaxParser.Parse(tokens);

                if (logger.IsEmpty)
                    syntaxNode.Accept(evaluator);
                
                handler.Handle(logger);
                logger.Reset();
            }
        }
    }
}