using MSMQHandlerService.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IQueueServiceEws
    {
        Task<string> DeleteFolderQueueAsync(EwsDeleteFolderQueue ewsDeleteFolderQueue, CancellationToken token);
        Task<string> GetEmailQueueAsync(EwsGetEmailQueue ewsGetEmailQueue, CancellationToken token);
        Task<string> GetEmailsQueueAsync(EwsGetEmailsQueue ewsGetEmailsQueue, CancellationToken token);
        Task<string> GetFolderQueueAsync(EwsGetFolderQueue ewsGetFolderQueue, CancellationToken token);
        Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false);
    }
}