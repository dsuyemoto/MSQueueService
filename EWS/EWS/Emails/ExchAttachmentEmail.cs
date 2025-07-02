using Microsoft.Exchange.WebServices.Data;

namespace EWS
{
    public class ExchAttachmentEmail : IExchAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public ExchEmail ExchEmail { get; set; }

        public ExchAttachmentEmail()
        {

        }

        public ExchAttachmentEmail(ItemAttachment itemAttachment)
        {
            var emailMessage = itemAttachment.Item as EmailMessage;
            FileName = itemAttachment.Name;
            Content = emailMessage.MimeContent.Content;
            ExchEmail = new ExchEmail(emailMessage, true);
        }
    }
}
