using System.Globalization;
using Ninject;
using Ninject.Extensions.Conventions;

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

            container.Bind<IConsolePrinter>().To<ConsolePrinter>().InSingletonScope();
            
            container.Bind(conf => conf
                .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ICommand>()
                .BindAllInterfaces());
            
            return container.Get<Repl>();
        }
    }
}