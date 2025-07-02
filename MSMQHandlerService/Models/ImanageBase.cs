namespace MSMQHandlerService.Models
{
    public abstract class ImanageBase
    {
        public int MaxRetries { get; set; }
        public int MaxWaitTimeSeconds { get; set; } = 5;
    }
}