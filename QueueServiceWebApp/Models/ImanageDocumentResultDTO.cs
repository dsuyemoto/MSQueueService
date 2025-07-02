using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class ImanageDocumentResultDTO : ImanageDocumentSecurity
    {
        public string Author { get; set; }
        public string Operator { get; set; }
        public string DescriptionBase64 { get; set; }
        public string CommentBase64 { get; set; }
        public string Number { get; set; }
        public string Version { get; set; }        
        public ImanageDocumentNrlDTO ImanageNrl { get; set; }
        public ImanageErrorDTO ImanageError { get; set; }
        public string Extension { get; set; }

        public ImanageDocumentResultDTO()
        {

        }

        public ImanageDocumentResultDTO(ImanageDocumentQueue imanageDocumentQueue)
        {
            Author = imanageDocumentQueue.Author;
            CommentBase64 = imanageDocumentQueue.CommentBase64;
            Database = imanageDocumentQueue.Database;
            DescriptionBase64 = imanageDocumentQueue.DescriptionBase64;
            ImanageError = new ImanageErrorDTO(imanageDocumentQueue.ImanageError);
            ImanageNrl = new ImanageDocumentNrlDTO(imanageDocumentQueue.ImanageNrl);
            Operator = imanageDocumentQueue.Operator;
            Number = imanageDocumentQueue.Number;
            Version = imanageDocumentQueue.Version;
            Session = imanageDocumentQueue.Session;
            Extension = imanageDocumentQueue.Extension;
        }
    }
}