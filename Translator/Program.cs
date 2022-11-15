using System;
using System.Globalization;
using Ninject;
using Ninject.Extensions.Conventions;
using Translator.Core.Evaluation;
using Translator.Core.Lexing;
using Translator.Core.Lexing.TokenParsers;
using Translator.Core.Syntax;

namespace Translator
{
    public static class Program
    {
        private const string CoreAssembly = "Translator.Core";
        
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            
            var container = ConfigureContainer();
            var lexer = container.Get<Lexer>();
            var syntaxParser = container.Get<SyntaxParser>();
            var evaluator = container.Get<Evaluator>();

            while (true)
            {
                var code = Console.ReadLine();
                var tokens = lexer.Tokenize(code);
                var syntaxNode = syntaxParser.Parse(tokens);
            
                var value = syntaxNode.Accept(evaluator);
                Console.WriteLine(value);
            }
        }

        private static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();

            container.Bind<Lexer>().ToSelf();
            container.Bind<SyntaxParser>().ToSelf();
            container.Bind<Evaluator>().ToSelf();
            container.Bind(conf => conf
                .From(CoreAssembly)
                .SelectAllClasses()
                .InheritedFrom<ITokenParser>()
                .BindAllInterfaces());
            
            container.Bind(conf => conf
                .From(CoreAssembly)
                .SelectAllClasses()
                .InheritedFrom<IBinaryOperation>()
                .BindAllInterfaces());

            return container;
        }        
    }
}