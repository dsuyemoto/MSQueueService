namespace MSMQHandlerService.Models
{
    public class EwsResultFolderQueue
    {
        public EwsFolderQueue Folder { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public EwsResultFolderQueue()
        {

        }
    }
}