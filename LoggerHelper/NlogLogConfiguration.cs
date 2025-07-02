namespace LoggerHelper
{
    public class NlogLogConfiguration : LogConfigurationBase
    {
        public string InternalLogLevel { get; private set; }
        public string InternalLoggingFile { get; private set; }
        public bool InternalLoggingToConsole { get; private set; }
        public NlogTargetConfigurationBase[] TargetConfigurations { get; private set; }

        public NlogLogConfiguration(
            string logLevel,
            string appDomain,
            NlogTargetConfigurationBase[] targetConfigurations,
            string internalLogLevel = "",
            string internalLoggingFile = "",
            bool internalLoggingToConsole = false) : base(
                logLevel,
                appDomain)
        {
            InternalLoggingFile = internalLoggingFile;
            InternalLoggingToConsole = internalLoggingToConsole;
            InternalLogLevel = internalLogLevel;
            TargetConfigurations = targetConfigurations;
        }
    }
}
