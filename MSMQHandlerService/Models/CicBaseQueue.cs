namespace MSMQHandlerService.Models
{
    public class CicBaseQueue
    {
        public CicCredsQueue Creds { get; set; }
        public int MaxRetries { get; set; }
        public int MaxWaitTimeSeconds { get; set; } = 5;
    }
}