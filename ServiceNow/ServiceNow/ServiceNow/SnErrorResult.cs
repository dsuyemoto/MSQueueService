using System;

namespace ServiceNow
{
    public class SnErrorResult
    {
        public string Detail { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

        public SnErrorResult()
        {

        }

        public SnErrorResult(ErrorDTO errorDTO)
        {
            if (errorDTO != null)
            {
                Detail = errorDTO.detail;
                Message = errorDTO.message;
                Status = errorDTO.status;
            }
        }

        public SnErrorResult(Exception ex)
        {
            Detail = ex.StackTrace;
            Message = ex.Message;
            Status = ex.Source;
        }
    }
}