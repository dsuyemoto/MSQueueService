namespace MSMQHandlerService.Models
{
    public class ImanageCreateDocumentQueue : ImanageBase
    {
        public string FolderId { get; set; }
        public ImanageDocumentQueue Document { get; set; }
        public string[] OutputProfileIds { get; set; }
        public ImanageSourceEmailQueue SourceEmail { get; set; }
        public string SourceFilePath { get; set; }

        public ImanageCreateDocumentQueue()
        {

        }
    }
}