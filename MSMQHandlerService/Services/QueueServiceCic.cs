using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class QueueServiceCic : QueueServiceBase, IQueueServiceCic
    {
        const string CICQUEUENAME = "Cic";
        const string CICRESPONSEQUEUENAME = "CicResponse";
        const string CICFAILEDQUEUENAME = "CicFailed";

        public QueueServiceCic(QueueType queueType, bool isRecoverable, ILogger logger) : base(logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            if (queueType != QueueType.MSMQ) throw new Exception("QueueType not defined");

            _queueService = new MSMQService(
                CICQUEUENAME,
                CICRESPONSEQUEUENAME,
                CICFAILEDQUEUENAME,
                new Type[] {
                        typeof(CicInteractionUpdateQueue),
                        typeof(CicServiceNowSourceQueue),
                        typeof(CicInteractionGetQueue)
                    },
                new Type[] {
                        typeof(CicInteractionResultQueue),
                        typeof(ErrorResultQueue)
                    },
                isRecoverable,
                _logger);
        }

        public async Task<string> GetInteractionQueueAsync(
            CicInteractionGetQueue cicInteractionGetQueue, 
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(cicInteractionGetQueue), token);
        }

        public async Task<string> UpdateInteractionQueueAsync(
            CicInteractionUpdateQueue cicInteractionUpdateQueue,
            CancellationToken token)
        {
            return await _queueService.RequestAsync(new MsmqMessage(cicInteractionUpdateQueue), token);
        }
    }
}