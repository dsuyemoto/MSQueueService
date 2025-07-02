namespace MSMQHandlerService.Models
{
    public class EwsEmailQueue : IEwsItem
    {
        public string FromName { get; set; }
        public string ToNames { get; set; }
        public string CcNames { get; set; }
        public string ReceivedDate { get; set; }
        public string SentDate { get; set; }
        public string Subject { get; set; }
        public string UniqueId { get; set; }
        public EwsFolderQueue EwsFolder { get; set; }
        public EwsEmailQueue[] Attachments { get; set; }

        public EwsEmailQueue()
        {

        }
    }
}