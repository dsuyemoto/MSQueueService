using System;

namespace Imanage.Documents
{
    public class EmailProfileItems
    {
        public string ToNames { get; set; }
        public string From { get; set; }
        public string CcNames { get; set; }
        public DateTime Sent { get; set; }
        public DateTime Received { get; set; }
        public string Subject { get; set; }

        public EmailProfileItems()
        {

        }

        public EmailProfileItems(
            string toNames,
            string from,
            string ccNames,
            DateTime sent,
            DateTime received,
            string subject)
        {
            ToNames = toNames;
            From = from;
            CcNames = ccNames;
            Sent = sent;
            Received = received;
            Subject = subject;
        }

        public EmailProfileItems(
            string toNames,
            string from,
            string ccNames, 
            string sent, 
            string received, 
            string subject)
        {
            ToNames = toNames;
            From = from;
            CcNames = ccNames;
            Sent = ConvertDateTime(sent);
            Received = ConvertDateTime(received);
            Subject = subject;
        }

        private DateTime ConvertDateTime(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime)) return DateTime.Now;

            DateTime convertedDateTime;
            if (DateTime.TryParse(dateTime, out convertedDateTime))
                return convertedDateTime;
            else
                throw new Exception("DateTime not in correct format");
        }
    }
}
