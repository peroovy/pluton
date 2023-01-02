using Core.Execution;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Core.Lexing.TokenParsers;
using Core.Logging;
using Core.Logging.Handlers;
using Core.Syntax;
using Core.Text;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Core
{
    public class Interpreter
    {
        public Interpreter(
            ITextParser textParser, 
            ILexer lexer, 
            ISyntaxParser syntaxParser, 
            IExecutor executor, 
            ILogger logger, ILogHandler logHandler)
        {
            TextParser = textParser;
            Lexer = lexer;
            SyntaxParser = syntaxParser;
            Executor = executor;
            Logger = logger;
            LogHandler = logHandler;
        }
        
        public ITextParser TextParser { get; }
        
        public ILexer Lexer { get; }
        
        public ISyntaxParser SyntaxParser { get; }
        
        public IExecutor Executor { get; }
        
        public ILogger Logger { get; }
        
        public ILogHandler LogHandler { get; }

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

        public static Interpreter Create()
        {
            var container = ConfigureContainer();

            return container.Get<Interpreter>();
        }
        
        public static StandardKernel ConfigureContainer()
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
            
            container.Bind(conf => conf
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<BuiltinFunction>()
                .BindBase());

            return container;
        }      
    }
}