namespace LoggerHelper
{
    public class DummyLogConfiguration : LogConfigurationBase
    {
        public DummyLogConfiguration(string logLevel = "", string appDomain = "") : base(logLevel, appDomain)
        {
        }
    }
}
