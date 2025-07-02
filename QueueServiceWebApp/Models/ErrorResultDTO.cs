using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class ErrorResultDTO
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }

        public ErrorResultDTO()
        {

        }

        public ErrorResultDTO(ErrorResultQueue errorResultQueue)
        {
            if (errorResultQueue == null) return;
            
            Message = errorResultQueue.Message;
            Detail = errorResultQueue.Detail;
            Status = errorResultQueue.Status;
        }
    }
}