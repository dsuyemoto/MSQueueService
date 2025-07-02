using MSMQHandlerService.Models;
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ImanageErrorDTO
    {
        public ImanageProfileErrorDTO[] ProfileErrors { get; set; }
        public string Message { get; set; }

        public ImanageErrorDTO()
        {

        }

        public ImanageErrorDTO(ImanageErrorQueue imanageErrorQueue)
        {
            var profileErrors = new List<ImanageProfileErrorDTO>();
            if (imanageErrorQueue != null)
            {
                if (imanageErrorQueue.ProfileErrors != null)
                    foreach (var profileError in imanageErrorQueue.ProfileErrors)
                        profileErrors.Add(new ImanageProfileErrorDTO(profileError));
                ProfileErrors = profileErrors.ToArray();
                Message = imanageErrorQueue.Message;
            }
        }
    }
}