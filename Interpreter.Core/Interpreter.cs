using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Operations.Binary;
using Interpreter.Core.Execution.Operations.Unary;
using Interpreter.Core.Lexing;
using Interpreter.Core.Lexing.TokenParsers;
using Interpreter.Core.Logging;
using Interpreter.Core.Logging.Handlers;
using Interpreter.Core.Syntax;
using Interpreter.Core.Text;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Interpreter.Core
{
    public class Interpreter
    {
        public Interpreter()
        {
            var container = Configure();
            Logger = container.Get<ILogger>();
            LogHandler = container.Get<ILogHandler>();

            TextParser = container.Get<ITextParser>();
            Lexer = container.Get<ILexer>();
            SyntaxParser = container.Get<ISyntaxParser>();
            Executor = container.Get<IExecutor>();
        }
        
        public ILogger Logger { get; }
        
        public ILogHandler LogHandler { get; }
        
        public ITextParser TextParser { get; }
        
        public ILexer Lexer { get; }
        
        public ISyntaxParser SyntaxParser { get; }
        
        public IExecutor Executor { get; }

        public void Interpret(string text)
        {
            var lines = TextParser.ParseLines(text);
            var tokens = Lexer.Tokenize(lines);
            var syntaxTree = SyntaxParser.Parse(tokens);

            if (Logger.IsEmpty)
                syntaxTree.Accept(Executor);
                
            LogHandler.Handle(Logger);
            Logger.Reset();
        }
        
        public static StandardKernel Configure()
        {
            var container = new StandardKernel();

            container.Bind<ILogger>().To<Logger>().InSingletonScope();
            container.Bind<ILogHandler>().To<ConsoleHandler>().InSingletonScope();

            container.Bind<ITextParser>().To<TextParser>().InSingletonScope();
            container.Bind<ILexer>().To<Lexer>().InSingletonScope();
            container.Bind<ISyntaxParser>().To<SyntaxParser>().InSingletonScope();
            container.Bind<IExecutor>().To<Executor>().InSingletonScope();
            
            container.Bind(conf => conf
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ITokenParser>()
                .BindAllInterfaces());
            
            container.Bind(conf => conf
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<BinaryOperation>()
                .BindBase());
            
            container.Bind(conf => conf
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<UnaryOperation>()
                .BindBase());

            return container;
        }      
    }
}