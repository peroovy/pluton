using Ninject;
using Ninject.Extensions.Conventions;
using Translator.Core.Execution;
using Translator.Core.Execution.Operations.Binary;
using Translator.Core.Execution.Operations.Unary;
using Translator.Core.Lexing;
using Translator.Core.Lexing.TokenParsers;
using Translator.Core.Logging;
using Translator.Core.Logging.Handlers;
using Translator.Core.Syntax;
using Translator.Core.Text;

namespace Translator.Core
{
    public static class Interpreter
    {
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