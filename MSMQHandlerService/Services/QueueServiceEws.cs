using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class QueueServiceEws : QueueServiceBase, IQueueServiceEws
    {
        const string EWSQUEUENAME = "Ews";
        const string EWSRESPONSEQUEUENAME = "EwsResponse";
        const string EWSFAILEDQUEUENAME = "EwsFailed";

        public QueueServiceEws(QueueType queueType, bool isRecoverable, ILogger logger) : base(logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            if (queueType != QueueType.MSMQ) throw new Exception("QueueType not defined");

            _queueService = new MSMQService(
                EWSQUEUENAME,
                EWSRESPONSEQUEUENAME,
                EWSFAILEDQUEUENAME,
                new Type[] {
                        typeof(EwsGetEmailQueue),
                        typeof(EwsGetEmailsQueue),
                        typeof(EwsGetFolderQueue),
                        typeof(EwsDeleteFolderQueue),
                        typeof(EwsFolderQueue),
                        typeof(EwsCredsQueue)
                    },
                new Type[] {
                        typeof(EwsResultEmailQueue),
                        typeof(EwsResultFolderQueue),
                        typeof(EwsEmailQueue),
                        typeof(EwsFolderQueue),
                        typeof(ErrorResultQueue)
                    },
                isRecoverable,
                logger);
        }

        public async Task<string> GetFolderQueueAsync(EwsGetFolderQueue ewsGetFolderQueue, CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(ewsGetFolderQueue), token);
        }

        public async Task<string> GetEmailsQueueAsync(EwsGetEmailsQueue ewsGetEmailsQueue, CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(ewsGetEmailsQueue), token);
        }

        public async Task<string> GetEmailQueueAsync(EwsGetEmailQueue ewsGetEmailQueue, CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(ewsGetEmailQueue), token);
        }

        public async Task<string> DeleteFolderQueueAsync(EwsDeleteFolderQueue ewsDeleteFolderQueue, CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(ewsDeleteFolderQueue), token);
        }
    }
}