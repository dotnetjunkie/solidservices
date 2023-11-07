namespace BusinessLayer
{
    public interface ILogger
    {
        void Log(string message);
    }

    public static class LoggerExtensions
    {
        public static void LogInformation(this ILogger logger, string messageTemplate, params object[] parameters)
        {
            // TODO: Use real structured logging here.
            logger.Log(messageTemplate);
        }
    }
}