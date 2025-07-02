namespace MSMQHandlerService.Models
{
    public class CicInteractionGetQueue : CicBaseQueue
    {
        public string InteractionId { get; set; }
        public string AttributeNames { get; set; }

        public CicInteractionGetQueue()
        {

        }
    }
}