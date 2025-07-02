using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class EwsFileDTO
    {
        public string UniqueId { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }

        public EwsFileDTO()
        {

        }

        public EwsFileDTO(EwsFileQueue ewsFileQueue)
        {
            UniqueId = ewsFileQueue.UniqueId;
            FileName = ewsFileQueue.FileName;
            Content = ewsFileQueue.Content;
        }
    }
}