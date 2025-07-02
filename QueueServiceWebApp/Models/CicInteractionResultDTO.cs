using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class CicInteractionResultDTO
    {
        public bool IsSuccessful { get; set; }
        public string InteractionId { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }

        public CicInteractionResultDTO()
        {

        }
    }
}