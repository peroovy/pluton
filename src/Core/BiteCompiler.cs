using Core.Lexing;
using Core.Lexing.TokenParsers;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Utils.Diagnostic;
using Core.Utils.Text;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Core
{
    public class BiteCompiler
    {
        private readonly ITextParser textParser;
        private readonly ILexer lexer;
        private readonly ISyntaxParser syntaxParser;
        private readonly IDiagnosticBag diagnosticBag;

        public BiteCompiler(ITextParser textParser, ILexer lexer, ISyntaxParser syntaxParser, IDiagnosticBag diagnosticBag)
        {
            this.textParser = textParser;
            this.lexer = lexer;
            this.syntaxParser = syntaxParser;
            this.diagnosticBag = diagnosticBag;
        }

        public TranslationState<SyntaxTree> Compile(string text)
        {
            diagnosticBag.Clear();
            
            var lines = textParser.ParseLines(text);
            var tokens = lexer.Tokenize(lines);
            var syntaxTree = syntaxParser.Parse(tokens);

            return new TranslationState<SyntaxTree>(syntaxTree, diagnosticBag.Copy());
        }

        public static BiteCompiler Create()
        {
            var container = ConfigureContainer();

            return container.Get<BiteCompiler>();
        }

        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();

            container.Bind<IDiagnosticBag>().To<DiagnosticBag>().InSingletonScope();

            container.Bind<ITextParser>().To<TextParser>().InSingletonScope();
            container.Bind<ILexer>().To<Lexer>().InSingletonScope();
            container.Bind<ISyntaxParser>().To<SyntaxParser>().InSingletonScope();
            
            container.Bind(conf => conf
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ITokenParser>()
                .BindAllInterfaces());

            return container;
        }
    }
}