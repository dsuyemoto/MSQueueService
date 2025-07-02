using MSMQHandlerService.Models;
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ImanageResultDocumentDTO
    {
        public List<ImanageDocumentResultDTO> Documents { get; set; }
        public string FolderId { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }

        public ImanageResultDocumentDTO()
        {

        }

        public ImanageResultDocumentDTO(ImanageResultDocumentQueue imanageResultDocumentQueue)
        {
            var documents = new List<ImanageDocumentResultDTO>();
            if (imanageResultDocumentQueue.Documents != null && imanageResultDocumentQueue.Documents.Length > 0)
                foreach (var document in imanageResultDocumentQueue.Documents)
                    documents.Add(new ImanageDocumentResultDTO(document));
            Documents = documents;
            FolderId = imanageResultDocumentQueue.FolderId;
            if (imanageResultDocumentQueue.ErrorResult != null)
                ErrorResult = new ErrorResultDTO(imanageResultDocumentQueue.ErrorResult);
        }
    }
}