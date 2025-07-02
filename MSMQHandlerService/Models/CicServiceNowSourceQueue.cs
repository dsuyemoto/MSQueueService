using System;

namespace MSMQHandlerService.Models
{
    [Serializable]
    public class CicServiceNowSourceQueue
    {
        public string[][] CicToSnNameMappings { get; set; }
        public string MessageIdBase64 { get; set; }

        public CicServiceNowSourceQueue()
        {

        }
    }
}