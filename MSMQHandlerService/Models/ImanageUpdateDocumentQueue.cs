namespace MSMQHandlerService.Models
{
    public class ImanageUpdateDocumentQueue : ImanageBase
    {
        public string MessageIdBase64 { get; set; }
        public ImanageDocumentQueue Document { get; set; }
        public string[] OutputProfileItems { get; set; }
        public ImanageSourceEmailQueue SourceEmail { get; set; }
        public string SourceFilePath { get; set; }
        public bool AttachmentsOnly { get; set; } = false;
        public string FolderId { get; set; }

        public ImanageUpdateDocumentQueue()
        {

        }
    }
}