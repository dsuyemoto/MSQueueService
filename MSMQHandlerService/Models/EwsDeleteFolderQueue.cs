namespace MSMQHandlerService.Models
{
    public class EwsDeleteFolderQueue : EwsBaseQueue
    {
        public string MessageIdBase64 { get; set; }
        public string UniqueId { get; set; }
        public string FolderName { get; set; }

        public EwsDeleteFolderQueue()
        {

        }
    }
}