using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class QueueServiceImanage : QueueServiceBase, IQueueServiceImanage
    {
        const string IMANAGEQUEUENAME = "Imanage";
        const string IMANAGERESPONSEQUEUENAME = "ImanageResponse";
        const string IMANAGEFAILEDQUEUENAME = "ImanageFailed";

        public QueueServiceImanage(QueueType queueType, bool isRecoverable, ILogger logger) : base(logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            if (queueType != QueueType.MSMQ) throw new Exception("QueueType not defined");

            _queueService = new MSMQService(
                IMANAGEQUEUENAME,
                IMANAGERESPONSEQUEUENAME,
                IMANAGEFAILEDQUEUENAME,
                new Type[] {
                    typeof(ImanageCreateDocumentQueue),
                    typeof(ImanageGetDocumentQueue),
                    typeof(ImanageUpdateDocumentQueue),
                    typeof(ImanageDocumentQueue),
                    typeof(ImanageGetWorkspacesQueue),
                    typeof(ImanageEmailPropertiesQueue),
                    typeof(EwsCredsQueue),
                    typeof(ImanageSourceEmailQueue),
                    typeof(ImanageWorkspaceGetQueue)
                },
                new Type[] {
                    typeof(ImanageResultDocumentQueue),
                    typeof(ImanageDocumentQueue),
                    typeof(ImanageDocumentNrlQueue),
                    typeof(ImanageEmailPropertiesQueue),
                    typeof(ImanageErrorQueue),
                    typeof(ImanageProfileErrorQueue),
                    typeof(ImanageWorkspaceGetQueue),
                    typeof(ErrorResultQueue)
                },
                isRecoverable,
                logger);
        }

        public async Task<string> GetDocumentQueueAsync(
            ImanageGetDocumentQueue imanageGetDocumentQueue, 
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(imanageGetDocumentQueue), token);
        }

        public async Task<string> CreateDocumentQueueAsync(
            ImanageCreateDocumentQueue imanageCreateDocumentQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(imanageCreateDocumentQueue), token);
        }

        public async Task<string> UpdateDocumentQueueAsync(
            ImanageUpdateDocumentQueue imanageUpdateDocumentQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(imanageUpdateDocumentQueue), token);
        }
    }
}