using MSMQHandlerService.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IQueueServiceCic
    {
        Task<string> GetInteractionQueueAsync(CicInteractionGetQueue cicInteractionGetQueue, CancellationToken token);
        Task<string> UpdateInteractionQueueAsync(CicInteractionUpdateQueue cicInteractionUpdateQueue, CancellationToken token);
        Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false);
    }
}