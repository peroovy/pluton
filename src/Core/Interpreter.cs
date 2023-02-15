using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Execution.DataModel.Objects.Functions.Builtin;
using Core.Execution.DataModel.Operations.Binary;
using Core.Execution.DataModel.Operations.Unary;
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
        public Interpreter(ILexer lexer, ISyntaxParser parser, IExecutor executor)
        {
            Lexer = lexer;
            Parser = parser;
            Executor = executor;
        }
        
        public ILexer Lexer { get; }
        
        public ISyntaxParser Parser { get; }
        
        public IExecutor Executor { get; }

        public TranslationState<SyntaxTree> Parse(string text)
        {
            var sourceText = new SourceText(text);
            var lexing = Lexer.Tokenize(sourceText);
            var parsing = Parser.Parse(sourceText, lexing.Result);

            var diagnostics = lexing.DiagnosticBag.Concat(parsing.DiagnosticBag);

            return new TranslationState<SyntaxTree>(parsing.Result, diagnostics);
        }

        public TranslationState<Obj> Execute(string text)
        {
            var parsing = Parse(text);
            if (parsing.HasErrors)
                return new TranslationState<Obj>(null, parsing.DiagnosticBag);
            
            var interpretation = Executor.Execute(parsing.Result);
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