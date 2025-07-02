namespace QueueServiceWebApp.Models
{
    public abstract class ImanageBaseDTO
    {
        public int MaxRetries { get; set; }
        public int MaxWaitTimeSeconds { get; set; } = 5;
    }
}