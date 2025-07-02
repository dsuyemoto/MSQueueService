namespace MSMQHandlerService.Models
{
    public class EwsBaseQueue
    {
        public EwsCredsQueue Creds { get; set; }
        public int MaxWaitTimeSeconds { get; set; }
    }
}