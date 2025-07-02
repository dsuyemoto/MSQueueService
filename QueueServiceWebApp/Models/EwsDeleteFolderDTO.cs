namespace QueueServiceWebApp.Models
{
    public class EwsDeleteFolderDTO : EwsBaseDTO
    {
        public string MessageIdBase64 { get; set; }
        public string UniqueId { get; set; }
        public string FolderName { get; set; }

        public EwsDeleteFolderDTO()
        {

        }
    }
}