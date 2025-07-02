namespace QueueServiceWebApp.Models
{
    public class EwsGetFolderDTO : EwsBaseDTO
    {
        public string FolderPath { get; set; }
        public string MailboxEmailAddress { get; set; }

        public EwsGetFolderDTO()
        {

        }
    }
}