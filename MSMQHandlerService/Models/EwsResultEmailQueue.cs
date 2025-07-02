namespace MSMQHandlerService.Models
{
    public class EwsResultEmailQueue
    {
        public EwsEmailQueue[] Emails { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public EwsResultEmailQueue()
        {

        }
    }
}