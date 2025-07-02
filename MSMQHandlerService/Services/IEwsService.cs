using EWS;
using MSMQHandlerService.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IEwsService
    {
        Task<object> HandlerCallbackAsync(object ewsObject, CancellationToken token);
        Task<byte[]> DownloadEmailAsync(string uniqueId, EwsCredsQueue ewsCredsQueue, CancellationToken token);
        Task<List<IExchAttachment>> DownloadAttachmentsAsync(
            EwsEmailQueue ewsEmailQueue,
            EwsCredsQueue ewsCredsQueue,
            CancellationToken token);
    }
}