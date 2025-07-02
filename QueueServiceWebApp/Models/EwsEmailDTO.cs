using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class EwsEmailDTO
    {
        public string FromName { get; set; }
        public string ToNames { get; set; }
        public string CcNames { get; set; }
        public string ReceivedDate { get; set; }
        public string SentDate { get; set; }
        public string Subject { get; set; }
        public string UniqueId { get; set; }
        public EwsFolderDTO EwsFolder { get; set; }

        public EwsEmailDTO()
        {
        }

        public EwsEmailDTO(EwsEmailQueue ewsEmail)
        {
            FromName = ewsEmail.FromName;
            ToNames = ewsEmail.ToNames;
            CcNames = ewsEmail.CcNames;
            ReceivedDate = ewsEmail.ReceivedDate;
            SentDate = ewsEmail.SentDate;
            Subject = ewsEmail.Subject;
            UniqueId = ewsEmail.UniqueId;
            EwsFolder = new EwsFolderDTO(ewsEmail.EwsFolder);
        }
    }
}