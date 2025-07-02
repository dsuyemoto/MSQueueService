namespace QueueServiceWebApp.Models
{
    public class ImanageSourceEmailDTO
    {
        public string MessageIdBase64 { get; set; }
        public string FolderUniqueId { get; set; }
        public string FolderName { get; set; }
        public EwsCredsDTO Creds { get; set; }
        public bool _DeleteSourceFolder { get; set; }
        public string[] Content { get; set; }

        public ImanageSourceEmailDTO()
        {

        }
    }
}