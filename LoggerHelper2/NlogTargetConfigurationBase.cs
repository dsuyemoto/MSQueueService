using NLog.Targets;

namespace LoggerHelper
{
    public abstract class NlogTargetConfigurationBase
    {
        public NlogTargetConfigurationBase()
        {
        }

        public abstract Target CreateTarget(string targetId);
    }
}
