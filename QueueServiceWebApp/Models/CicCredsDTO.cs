namespace QueueServiceWebApp.Models
{
    public class CicCredsDTO
    {
        public string Servername { get; set; }
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }

        public CicCredsDTO()
        {

        }
    }
}