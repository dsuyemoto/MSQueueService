using static QueueService.MsmqMessageQueue;

namespace QueueService
{
    public class MsmqQueuePermission
    {
        public string SecurityName { get; set; }
        public MsmqRights Rights { get; set; }

        public MsmqQueuePermission(string securityName, MsmqRights rights)
        {
            SecurityName = securityName;
            Rights = rights;
        }
    }
}
