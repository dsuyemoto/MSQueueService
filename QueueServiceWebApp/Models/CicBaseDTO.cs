namespace QueueServiceWebApp.Models
{
    public abstract class CicBaseDTO
    {
        public int MaxRetries { get; set; }
        public int MaxWaitTimeSeconds { get; set; } = 5;
        public string Servername { get; set; }
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }
    }
}