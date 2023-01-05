using Core.Execution;
using Core.Execution.Interrupts;
using Core.Execution.Objects;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Syntax.AST;
using Core.Utils.Diagnostic;
using Ninject;
using Ninject.Extensions.Conventions;

namespace Core
{
    public class Interpreter
    {
        private readonly IExecutor executor;
        private readonly IDiagnosticBag diagnosticBag;

        public Interpreter(IExecutor executor, IDiagnosticBag diagnosticBag)
        {
            this.executor = executor;
            this.diagnosticBag = diagnosticBag;
        }

        public TranslationState<Obj> Run(SyntaxTree syntaxTree)
        {
            diagnosticBag.Clear();

            try
            {
                var value = syntaxTree.Accept(executor);
                
                return new TranslationState<Obj>(value, diagnosticBag.Copy());
            }
            catch (RuntimeException)
            {
                return new TranslationState<Obj>(null, diagnosticBag.Copy());
            }
        }

        public static Interpreter Create()
        {
            var container = ConfigureContainer();

            return container.Get<Interpreter>();
        }
        
        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();

            container.Bind<IDiagnosticBag>().To<DiagnosticBag>().InSingletonScope();
            
            container.Bind<IExecutor>().To<Executor>().InSingletonScope();

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