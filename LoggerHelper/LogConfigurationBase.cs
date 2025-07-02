namespace LoggerHelper
{
    public abstract class LogConfigurationBase
    {
        public string LogLevel { get; protected set; }
        public string AppDomain { get; protected set; }

        protected LogConfigurationBase(
            string logLevel,
            string appDomain
            )
        {
            LogLevel = logLevel;
            AppDomain = appDomain;
        }
    }
}
