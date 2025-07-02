
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class CicServiceNowSourceDTO
    {
        public Dictionary<string, string> CicToSnNameMappings { get; set; }
        public string MessageIdBase64 { get; set; }

        public CicServiceNowSourceDTO()
        {

        }
    }
}