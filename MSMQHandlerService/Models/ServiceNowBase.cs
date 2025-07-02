namespace MSMQHandlerService.Models
{
    public abstract class ServiceNowBase
    {
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }
        public string InstanceUrl { get; set; }
        public string _ResultNames { get; set; }
        public int _MaxRetries { get; set; } = 5;
        public string _SysId { get; set; }
    }
}