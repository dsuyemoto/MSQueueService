using Imanage.Documents;

namespace MSMQHandlerService.Models
{
    public class ImanageEmailPropertiesQueue
    {
        public string FromName { get; set; }
        public string ToNames { get; set; }
        public string CcNames { get; set; }
        public string ReceivedDate { get; set; }
        public string SentDate { get; set; }
        public string Subject { get; set; }

        public ImanageEmailPropertiesQueue()
        {

        }

        public ImanageEmailPropertiesQueue(EmailProfileItems emailProfileItems)
        {
            FromName = emailProfileItems.From;
            ToNames = emailProfileItems.ToNames;
            CcNames = emailProfileItems.CcNames;
            ReceivedDate = emailProfileItems.Received.ToString();
            SentDate = emailProfileItems.Sent.ToString();
            Subject = emailProfileItems.Subject;
        }
    }
}