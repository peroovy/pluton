using System;
using Ninject;
using Ninject.Extensions.Conventions;
using Translator.Core.Lexing;
using Translator.Core.Lexing.TokenParsers;

namespace Translator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var container = ConfigureContainer();
            var lexer = container.Get<Lexer>();

            var code = "2 + 2 *.3";
            var tokens = lexer.Tokenize(code);

            foreach (var token in tokens)
                Console.WriteLine(token);
        }

        private static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();

            container.Bind<Lexer>().ToSelf();
            container.Bind(conf => conf
                .FromAssembliesMatching("Translator.Core")
                .SelectAllClasses()
                .InheritedFrom<ITokenParser>()
                .BindAllInterfaces());

            return container;
        }        
    }
}