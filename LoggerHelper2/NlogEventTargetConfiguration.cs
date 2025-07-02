using NLog.Targets;

namespace LoggerHelper
{
    public class NlogEventTargetConfiguration : NlogTargetConfigurationBase
    {
        string _eventSourceName;
        int _applicationEventId;

        //eventSourceName is the name of the application that will be registered in the Windows Event Log.
        //applicationEventId is the ID number associated with the application event in the Windows Event Log.
        public NlogEventTargetConfiguration(int applicationEventId) : base()
        {
            _applicationEventId = applicationEventId;
            _eventSourceName = "UnknownApplication"; // Default value, can be overridden later
        }

        public override Target CreateTarget(string targetId)
        {
            return new EventLogTarget(targetId)
            {
                EventId = _applicationEventId.ToString(),
                Layout = "${message}",
                Log = "Application",
                Source = _eventSourceName
            };
        }

        public Target CreateTarget(string targetId, string appDomain)
        {
            _eventSourceName = appDomain;
            return CreateTarget(targetId);
        }
    }
}
