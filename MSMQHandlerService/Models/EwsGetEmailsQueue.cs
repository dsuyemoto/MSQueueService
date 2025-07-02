namespace MSMQHandlerService.Models
{
    public class EwsGetEmailsQueue : EwsBaseQueue
    {
        public string FolderMessageIdBase64 { get; set; }
        public EwsFolderQueue Folder { get; set; }


        public EwsGetEmailsQueue()
        {

        }
    }
}