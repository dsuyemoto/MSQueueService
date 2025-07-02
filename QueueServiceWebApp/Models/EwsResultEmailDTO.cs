using MSMQHandlerService.Models;
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class EwsResultEmailDTO
    {
        public EwsEmailDTO[] Emails { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }

        public EwsResultEmailDTO()
        {

        }

        public EwsResultEmailDTO(EwsResultEmailQueue ewsResultEmailQueue)
        {
            var emails = new List<EwsEmailDTO>();
            foreach (var ewsEmail in ewsResultEmailQueue.Emails)
                emails.Add(new EwsEmailDTO(ewsEmail));

            Emails = emails.ToArray();
            ErrorResult = new ErrorResultDTO(ewsResultEmailQueue.ErrorResult);
        }
    }
}