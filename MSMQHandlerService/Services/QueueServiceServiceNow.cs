using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class QueueServiceServiceNow : QueueServiceBase, IQueueServiceServiceNow
    {
        const string SERVICENOWQUEUENAME = "ServiceNow";
        const string SERVICENOWRESPONSEQUEUENAME = "ServiceNowResponse";
        const string SERVICENOWFAILEDQUEUENAME = "ServiceNowFailed";

        public QueueServiceServiceNow(QueueType queueType, bool isRecoverable, ILogger logger) : base(logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            if (queueType != QueueType.MSMQ) throw new Exception("QueueType not defined");

            _queueService = new MSMQService(
                SERVICENOWQUEUENAME,
                SERVICENOWRESPONSEQUEUENAME,
                SERVICENOWFAILEDQUEUENAME,
                new Type[] {
                        typeof(ServiceNowCreateTicketQueue),
                        typeof(ServiceNowGetTicketsQueue),
                        typeof(ServiceNowUpdateTicketQueue),
                        typeof(ServiceNowInsertImanageLinkQueue),
                        typeof(ServiceNowGetUserQueue),
                        typeof(ServiceNowQueryUserQueue),
                        typeof(ServiceNowQueryTicketQueue),
                        typeof(ServiceNowCreateAttachmentQueue),
                        typeof(ServiceNowAttachmentQueue),
                        typeof(ServiceNowSourceContentQueue),
                        typeof(ServiceNowQueryGroupQueue),
                        typeof(ServiceNowGetGroupQueue)
                },
                new Type[] {
                        typeof(ServiceNowResultTicketQueue),
                        typeof(ServiceNowTicketQueue),
                        typeof(ServiceNowUserQueue),
                        typeof(ServiceNowAttachmentQueue),
                        typeof(ServiceNowGroupQueue),
                        typeof(ServiceNowResultUserQueue),
                        typeof(ServiceNowResultAttachmentQueue),
                        typeof(ServiceNowResultGroupQueue),
                        typeof(ErrorResultQueue) 
                },
                isRecoverable,
                logger);
        }

        public async Task<string> CreateTicketQueueAsync(
            ServiceNowCreateTicketQueue serviceNowCreateTicketQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowCreateTicketQueue), token);
        }

        public async Task<string> GetTicketQueueAsync(
            ServiceNowGetTicketsQueue serviceNowGetTicketQueue, 
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowGetTicketQueue), token);
        }

        public async Task<string> UpdateTicketQueueAsync(
            ServiceNowUpdateTicketQueue serviceNowUpdateTicketQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowUpdateTicketQueue), token);
        }

        public async Task<string> CreateAttachmentQueueAsync(
            ServiceNowCreateAttachmentQueue serviceNowCreateAttachmentQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowCreateAttachmentQueue), token);
        }
        
        public async Task<string> GetUserQueueAsync(
            ServiceNowGetUserQueue serviceNowGetUserQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowGetUserQueue), token);
        }

        public async Task<string> QueryUserQueueAsync(
            ServiceNowQueryUserQueue serviceNowQueryUserQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowQueryUserQueue), token);
        }

        public async Task<string> QueryTicketQueueAsync(
            ServiceNowQueryTicketQueue serviceNowQueryTicketQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowQueryTicketQueue), token);
        }

        public async Task<string> QueryGroupQueueAsync(
            ServiceNowQueryGroupQueue serviceNowQueryGroupQueue, 
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowQueryGroupQueue), token);
        }

        public async Task<string> GetGroupQueueAsync(
            ServiceNowGetGroupQueue serviceNowGetGroupQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(serviceNowGetGroupQueue), token);
        }
    }
}