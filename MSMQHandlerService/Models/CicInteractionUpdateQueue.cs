namespace MSMQHandlerService.Models
{
    public class CicInteractionUpdateQueue : CicBaseQueue
    {
        public string MessageIdBase64 { get; set; }
        public string[][] Attributes { get; set; }
        public CicServiceNowSourceQueue SourceAttributes { get; set; }

        public CicInteractionUpdateQueue()
        {

        }
    }
}