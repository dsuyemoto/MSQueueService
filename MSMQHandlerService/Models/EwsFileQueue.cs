namespace MSMQHandlerService.Models
{
    public class EwsFileQueue : IEwsItem
    {
        public string UniqueId { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }

        public EwsFileQueue()
        {

        }
    }
}