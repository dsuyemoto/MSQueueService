namespace MSMQHandlerService.Models
{
    public class EwsGetFolderQueue : EwsBaseQueue
    {
        public string FolderPath { get; set; }
        public string MailboxEmailAddress { get; set; }

        public EwsGetFolderQueue()
        {

        }
    }
}