using MSMQHandlerService.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IQueueServiceServiceNow
    {
        Task<string> CreateAttachmentQueueAsync(ServiceNowCreateAttachmentQueue serviceNowCreateTicketAttachmentQueue, CancellationToken token);
        Task<string> CreateTicketQueueAsync(ServiceNowCreateTicketQueue serviceNowCreateTicketQueue, CancellationToken token);
        Task<string> GetTicketQueueAsync(ServiceNowGetTicketsQueue serviceNowGetTicketQueue, CancellationToken token);
        Task<string> UpdateTicketQueueAsync(ServiceNowUpdateTicketQueue serviceNowUpdateTicketQueue, CancellationToken token);
        Task<string> GetUserQueueAsync(ServiceNowGetUserQueue serviceNowGetUserQueue, CancellationToken token);
        Task<string> QueryUserQueueAsync(ServiceNowQueryUserQueue serviceNowQueryUserQueue, CancellationToken token);
        Task<string> QueryTicketQueueAsync(ServiceNowQueryTicketQueue serviceNowQueryTicketQueue, CancellationToken token);
        Task<string> QueryGroupQueueAsync(ServiceNowQueryGroupQueue serviceNowQueryGroupQueue, CancellationToken token);
        Task<string> GetGroupQueueAsync(ServiceNowGetGroupQueue serviceNowGetGroupQueue, CancellationToken token);
        Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false);
    }
}