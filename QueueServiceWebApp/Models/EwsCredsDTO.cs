using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class EwsCredsDTO
    {
        public string AutodiscoverEmailAddress { get; set; }
        public string Username { get; set; }
        public string PasswordBase64 { get; set; }

        public EwsCredsDTO()
        {

        }

        public EwsCredsDTO(EwsCredsQueue ewsCredsQueue)
        {
            AutodiscoverEmailAddress = ewsCredsQueue.AutodiscoverEmailAddress;
            Username = ewsCredsQueue.Username;
            PasswordBase64 = ewsCredsQueue.PasswordBase64;
        }
    }
}