using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class CicInteractionUpdateDTO : CicBaseDTO
    {
        public Dictionary<string, string> Attributes { get; set; }
        public CicServiceNowSourceDTO ServiceNowSource { get; set; }

        public CicInteractionUpdateDTO()
        {

        }
    }
}