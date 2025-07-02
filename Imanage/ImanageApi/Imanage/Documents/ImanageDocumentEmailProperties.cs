using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imanage
{
    public class ImanageDocumentEmailProperties
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public DateTime Received { get; set; }
        public DateTime Sent { get; set; }

        public ImanageDocumentEmailProperties(string from, string to, string cc, string subject, DateTime received, DateTime sent)
        {
            From = from;
            To = to;
            Cc = cc;
            Subject = subject;
            Received = received;
            Sent = sent;
        }
        public ImanageDocumentEmailProperties()
        {

        }
    }
}
