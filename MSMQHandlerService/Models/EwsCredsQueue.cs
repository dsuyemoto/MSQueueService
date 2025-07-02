namespace MSMQHandlerService.Models
{
    public class EwsCredsQueue
    {
        public string AutodiscoverEmailAddress { get; set; }
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }

        public EwsCredsQueue()
        {

        }
    }
}