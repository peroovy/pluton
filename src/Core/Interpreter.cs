using Core.Execution;
using Core.Execution.Objects;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Core.Lexing.TokenParsers;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Utils.Text;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Core
{
    public class Interpreter
    {
        private readonly ILexer lexer;
        private readonly ISyntaxParser parser;
        private readonly IExecutor executor;

        public Interpreter(ILexer lexer, ISyntaxParser parser, IExecutor executor)
        {
            this.lexer = lexer;
            this.parser = parser;
            this.executor = executor;
        }

        public TranslationState<SyntaxTree> Parse(string text)
        {
            var sourceText = new SourceText(text);
            var lexing = lexer.Tokenize(sourceText);
            var parsing = parser.Parse(sourceText, lexing.Result);

            var diagnostics = lexing.DiagnosticBag.Concat(parsing.DiagnosticBag);

            return new TranslationState<SyntaxTree>(parsing.Result, diagnostics);
        }

        public TranslationState<Obj> Execute(string text)
        {
            var parsing = Parse(text);
            if (parsing.HasErrors)
                return new TranslationState<Obj>(null, parsing.DiagnosticBag);
            
            var interpretation = executor.Execute(parsing.Result);
            var diagnostics = parsing.DiagnosticBag.Concat(interpretation.DiagnosticBag);

            return new TranslationState<Obj>(interpretation.Result, diagnostics);
        }

        public static Interpreter Create()
        {
            var container = ConfigureContainer();

            return container.Get<Interpreter>();
        }
        
        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();

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