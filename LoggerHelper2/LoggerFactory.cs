using System.Data;

namespace LoggerHelper
{
    public static class LoggerFactory
    {
        public enum LoggerType
        {
            Nlog,
            ConsoleOnly
        }

        public static ILogger GetLogger(LogConfigurationBase loggerConfiguration)
        {
            if (loggerConfiguration is NlogLogConfiguration)
                return new NlogLoggerWrapper(loggerConfiguration as NlogLogConfiguration);
            else if (loggerConfiguration is ConsoleLogConfiguration)
                return new ConsoleLogger(loggerConfiguration as ConsoleLogConfiguration);
            else
                return new DummyLogger();
        }
    }
}
