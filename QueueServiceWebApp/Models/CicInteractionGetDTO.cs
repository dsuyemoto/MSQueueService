namespace QueueServiceWebApp.Models
{
    public class CicInteractionGetDTO : CicBaseDTO
    {
        public string InteractionId { get; set; }
        public string AttributeNames { get; set; }

        public CicInteractionGetDTO()
        {

        }
    }
}