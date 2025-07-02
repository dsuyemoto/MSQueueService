using Imanage;
using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ImanageErrorQueue
    {
        public ImanageProfileErrorQueue[] ProfileErrors { get; set; }
        public string Message { get; set; }

        public ImanageErrorQueue()
        {

        }

        public ImanageErrorQueue(ImanageError imanageError)
        {
            if (imanageError == null) return;

            if (imanageError.ImanageProfileErrors != null)
            {
                var imanageErrors = new List<ImanageProfileErrorQueue>();
                foreach (var imanageProfileError in imanageError.ImanageProfileErrors)
                    imanageErrors.Add(new ImanageProfileErrorQueue(imanageProfileError));

                ProfileErrors = imanageErrors.ToArray();
            }

            Message = imanageError.Message;
        }
    }
}