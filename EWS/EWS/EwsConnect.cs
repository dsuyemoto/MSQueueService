using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace EWS
{
    public class EwsConnect
    {
        public string AutodiscoverEmailAddress { get; set; }
        public WebCredentials ServiceAccountCredentials { get; set; }
        public string TraceFolderPath { get; set; }
        public bool IsTraceEnabled { get; set; }

        public EwsConnect(
            string autodiscoverEmailAddress,
            NetworkCredential serviceAccountCredentials = null, 
            string traceFolderPath = "", 
            bool isTraceEnabled = false)
        {
            AutodiscoverEmailAddress = autodiscoverEmailAddress;
            ServiceAccountCredentials = new WebCredentials(serviceAccountCredentials);
            TraceFolderPath = traceFolderPath;
            IsTraceEnabled = isTraceEnabled;
        }
    }
}
