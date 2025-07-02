using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class ImanageDocumentNrlDTO
    {
        public string ContentBytesBase64 { get; set; }
        public string FileName { get; set; }

        public ImanageDocumentNrlDTO()
        {

        }

        public ImanageDocumentNrlDTO(ImanageDocumentNrlQueue imanageDocumentNrlQueue)
        {
            if (imanageDocumentNrlQueue != null)
            {
                ContentBytesBase64 = imanageDocumentNrlQueue.ContentBytesBase64;
                FileName = imanageDocumentNrlQueue.FileName;
            }
        }
    }
}