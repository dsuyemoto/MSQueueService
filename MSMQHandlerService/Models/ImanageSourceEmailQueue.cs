namespace MSMQHandlerService.Models
{
    public class ImanageSourceEmailQueue
    {
        public string MessageIdBase64 { get; set; }
        public string FolderUniqueId { get; set; }
        public string FolderName { get; set; }
        public EwsCredsQueue Creds { get; set; }
        public bool DeleteSourceFolder { get; set; }
        public string[] Content { get; set; } = new string[] { "email" };

        public ImanageSourceEmailQueue()
        {

        }
    }
}