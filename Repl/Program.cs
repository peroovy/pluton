using System.Globalization;
using Ninject;

namespace Repl
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            var repl = ConfigureRepl();
            repl.Run();
        }

        private static Repl ConfigureRepl()
        {
            var container = new StandardKernel();

            container.Bind<IDiagnosticPrinter>().To<DiagnosticPrinter>().InSingletonScope();
            
            return container.Get<Repl>();
        }
    }
}