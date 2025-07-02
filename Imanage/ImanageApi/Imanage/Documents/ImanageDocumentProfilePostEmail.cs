using Newtonsoft.Json;
using System;

namespace Imanage.Documents
{
    public class ImanageDocumentProfilePostEmail : ImanageDocumentProfilePost
    {
        [JsonProperty("custom13")]
        public string From { get; set; } //From
        [JsonProperty("custom14")]
        public string To { get; set; } //To
        [JsonProperty("custom15")]
        public string Cc { get; set; } //Cc
        [JsonProperty("custom21")]
        public DateTime SentDate { get; set; } // Sent Date
        [JsonProperty("custom22")]
        public DateTime ReceivedDate { get; set; } // Received Date

        public ImanageDocumentProfilePostEmail(DocumentProfileItems documentProfileItems) : base(documentProfileItems)
        {
            From = documentProfileItems.EmailProfileItems.From;
            To = documentProfileItems.EmailProfileItems.ToNames;
            Cc = documentProfileItems.EmailProfileItems.CcNames;
            SentDate = documentProfileItems.EmailProfileItems.Sent;
            ReceivedDate = documentProfileItems.EmailProfileItems.Received;
        }
    }
}
