using NLog.Targets;

namespace LoggerHelper
{
    public class NlogConsoleTargetConfiguration : NlogTargetConfigurationBase
    {
        public NlogConsoleTargetConfiguration() : base()
        {
        }

        public override Target CreateTarget(string targetName)
        {
            var consoleTarget = new ColoredConsoleTarget(targetName)
            {
                Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception}"
            };

            return consoleTarget;
        }
    }
}
