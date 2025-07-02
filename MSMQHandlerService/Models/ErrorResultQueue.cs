using ServiceNow;
using System;

namespace MSMQHandlerService.Models
{
    public class ErrorResultQueue
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }

        public ErrorResultQueue()
        {

        }

        public ErrorResultQueue(Exception ex)
        {
            Message = ex.Message;
            Detail = ex.StackTrace;
            Status = ex.HResult.ToString();
        }

        public ErrorResultQueue(string message)
        {
            Message = message;
        }

        public ErrorResultQueue(string[] errors)
        {
            Message = string.Join(",", errors);
        }

        public ErrorResultQueue(SnErrorResult snErrorResult)
        {
            if (snErrorResult is null)
                return;

            Message = snErrorResult.Message;
            Detail = snErrorResult.Detail;
            Status = snErrorResult.Status;
        }
    }
}