using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public abstract class ServiceNowBaseDTO
    {
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }
        public string InstanceUrl { get; set; }
        public string _ResultNames { get; set; }
        public int _MaxRetries { get; set; } = 5;
    }
}