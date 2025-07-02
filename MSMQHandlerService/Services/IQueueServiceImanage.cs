using MSMQHandlerService.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IQueueServiceImanage
    {
        Task<string> GetDocumentQueueAsync(ImanageGetDocumentQueue imanageDocumentGetDTO, CancellationToken token);
        Task<string> CreateDocumentQueueAsync(ImanageCreateDocumentQueue imanageDocumentCreateDTO, CancellationToken token);
        Task<string> UpdateDocumentQueueAsync(ImanageUpdateDocumentQueue imanageDocumentUpdateQueue, CancellationToken token);
        Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false);
    }
}