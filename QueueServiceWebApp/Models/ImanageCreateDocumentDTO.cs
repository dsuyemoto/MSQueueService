
namespace QueueServiceWebApp.Models
{
    public class ImanageCreateDocumentDTO : ImanageBaseDTO
    {
        public string FolderId { get; set; }
        public ImanageDocumentCreateDTO Document { get; set; }
        public string[] OutputProfileIds { get; set; }
        public ImanageSourceEmailDTO _SourceEmail { get; set; }
        public string _SourceFilePath { get; set; }

        public ImanageCreateDocumentDTO()
        {
        }
    }
}