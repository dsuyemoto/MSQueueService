namespace MSMQHandlerService.Models
{
    public class CicCredsQueue
    {
        public string Servername { get; set; }
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }

        public CicCredsQueue()
        {

        }
    }
}