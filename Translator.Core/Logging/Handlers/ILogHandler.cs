namespace Translator.Core.Logging.Handlers
{
    public interface ILogHandler
    {
        void Handle(ILogger logger);
    }
}