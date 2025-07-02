namespace LoggerHelper
{
    public class ConsoleLogConfiguration : LogConfigurationBase
    {
        public bool IsEnabled { get; private set; }

        public ConsoleLogConfiguration(
            string logLevel,
            string appDomain,
            bool isEnabled) : base(
                logLevel,
                appDomain)
        {
            IsEnabled = isEnabled;
        }
    }
}
