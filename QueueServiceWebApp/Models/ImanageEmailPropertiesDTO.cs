namespace QueueServiceWebApp.Models
{
    public class ImanageEmailPropertiesDTO
    {
        public string FromName { get; set; }
        public string ToNames { get; set; }
        public string CcNames { get; set; }
        public string ReceivedDate { get; set; }
        public string SentDate { get; set; }
        public string Subject { get; set; }

        public ImanageEmailPropertiesDTO()
        {

        }
    }
}