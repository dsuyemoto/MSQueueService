namespace QueueServiceWebApp.Models
{
    public class EwsGetEmailsDTO : EwsBaseDTO
    {
        public string FolderMessageIdBase64 { get; set; }
        public EwsFolderDTO Folder { get; set; }

        public EwsGetEmailsDTO()
        {

        }
    }
}