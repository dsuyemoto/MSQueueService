namespace MSMQHandlerService.Models
{
    public class CicInteractionResultQueue
    {
        public bool IsSuccessful { get; set; }
        public string InteractionId { get; set; }
        public object[][] Attributes { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public CicInteractionResultQueue()
        {

        }
    }
}