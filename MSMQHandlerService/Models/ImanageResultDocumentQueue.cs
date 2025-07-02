using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ImanageResultDocumentQueue
    {
        public ImanageDocumentQueue[] Documents { get; set; }
        public string FolderId { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public ImanageResultDocumentQueue()
        {

        }

        public ImanageResultDocumentQueue(
            ImanageDocumentQueue[] imanageDocumentQueues, 
            string folderId,
            ErrorResultQueue errorResultQueue)
        {
            Documents = imanageDocumentQueues;
            FolderId = folderId;
            ErrorResult = errorResultQueue;
        }
    }
}