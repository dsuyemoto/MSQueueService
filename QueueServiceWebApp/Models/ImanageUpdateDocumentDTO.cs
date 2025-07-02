namespace QueueServiceWebApp.Models
{
    public class ImanageUpdateDocumentDTO
    {       
        public ImanageDocumentUpdateDTO Document { get; set; }
        public string[] _OutputProfileItems { get; set; }
        public ImanageSourceEmailDTO _SourceEmail { get; set; }
        public string _SourceFilePath { get; set; }
        public bool _AttachmentsOnly { get; set; } = false;
        public string FolderId { get; set; }

        public ImanageUpdateDocumentDTO()
        {

        }
    }
}