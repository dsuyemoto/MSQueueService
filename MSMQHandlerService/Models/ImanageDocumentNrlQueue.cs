using Imanage;
using System;

namespace MSMQHandlerService.Models
{
    public class ImanageDocumentNrlQueue
    {
        public string ContentBytesBase64 { get; set; }
        public string FileName { get; set; }

        public ImanageDocumentNrlQueue()
        {

        }

        public ImanageDocumentNrlQueue(string contentBytesBase64, string fileName)
        {
            ContentBytesBase64 = contentBytesBase64;
            FileName = fileName;
        }

        public ImanageDocumentNrlQueue(ImanageDocumentNrl imanageDocumentNrl)
        {
            ContentBytesBase64 = Helpers.Base64ConvertTo(imanageDocumentNrl.Data);
            FileName = imanageDocumentNrl.FileName;
        }
    }
}