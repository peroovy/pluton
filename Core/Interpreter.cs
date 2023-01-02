using Core.Diagnostic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Core.Lexing.TokenParsers;
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
            IDiagnosticBag diagnosticBag)
        {
            TextParser = textParser;
            Lexer = lexer;
            SyntaxParser = syntaxParser;
            Executor = executor;
            DiagnosticBag = diagnosticBag;
        }
        
        public ITextParser TextParser { get; }
        
        public ILexer Lexer { get; }
        
        public ISyntaxParser SyntaxParser { get; }
        
        public IExecutor Executor { get; }
        
        public IDiagnosticBag DiagnosticBag { get; }

        public Obj Execute(string text)
        {
            var lines = TextParser.ParseLines(text);
            var tokens = Lexer.Tokenize(lines);
            var syntaxTree = SyntaxParser.Parse(tokens);

            if (!DiagnosticBag.IsEmpty)
                throw new CompilationException();

            return syntaxTree.Accept(Executor);
        }

        public void Reset()
        {
            DiagnosticBag.Reset();
        }

        public static Interpreter Create()
        {
            var container = CreateContainer();

            return container.Get<Interpreter>();
        }
        
        public static StandardKernel CreateContainer()
        {
            var container = new StandardKernel();

            container.Bind<IDiagnosticBag>().To<DiagnosticBag>().InSingletonScope();

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