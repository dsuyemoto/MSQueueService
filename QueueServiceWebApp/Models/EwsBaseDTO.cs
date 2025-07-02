namespace QueueServiceWebApp.Models
{
    public class EwsBaseDTO
    {
        public EwsCredsDTO Creds { get; set; }
        public int MaxWaitTimeSeconds { get; set; }
    }
}