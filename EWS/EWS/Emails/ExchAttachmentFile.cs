using Microsoft.Exchange.WebServices.Data;

namespace EWS
{
    public class ExchAttachmentFile : IExchAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string UniqueId { get; set; }

        public ExchAttachmentFile()
        {

        }

        public ExchAttachmentFile(string fileName, byte[] content, string uniqueId)
        {
            FileName = fileName;
            Content = content;
            UniqueId = uniqueId;
        }

        public ExchAttachmentFile(FileAttachment fileattachment)
        {
            FileName = fileattachment.Name;
            Content = fileattachment.Content;
            UniqueId = fileattachment.Id;
        }
    }
}
