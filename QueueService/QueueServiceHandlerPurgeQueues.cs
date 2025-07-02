using LoggerHelper;
using System;
using System.Timers;

namespace QueueService
{
    public class QueueServiceHandlerPurgeQueues : QueueServiceHandlerBase
    {
        protected Timer _timer;
        ILogger _logger;

        TimeSpan _purgeInterval;
        TimeSpan _messageExpiration;

        public QueueServiceHandlerPurgeQueues(
            IQueueService queueService, 
            TimeSpan messageExpiration,
            TimeSpan purgeInterval,
            ILogger logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            _queueService = queueService;
            _purgeInterval = purgeInterval;
            _messageExpiration = messageExpiration;
            _logger.Info("Initializing purge handler " + queueService.RequestQueueName);
            _logger.Info(
                $"Purge Interval for {queueService.RequestQueueName}: {_purgeInterval.TotalHours} hrs," +
                $" {_purgeInterval.Minutes } mins," +
                $" {_purgeInterval.Seconds} secs");
            _logger.Info($"Purge interval: {_purgeInterval.TotalMilliseconds} ms");

            _timer = new Timer(_purgeInterval.TotalMilliseconds);
            _timer.Elapsed += PurgeQueues;
        }

        public override void Start()
        {
            _logger.Info("Starting " + _queueService.RequestQueueName + " purge handler");

            _timer.Enabled = true;
        }

        public override void Stop()
        {
            _logger.Info("Stopping " + _queueService.RequestQueueName + " purge handler");

            _timer.Enabled = false;
        }

        private void PurgeQueues(object source, ElapsedEventArgs args)
        {
            _logger.Info("Purging message queues for " + _queueService.RequestQueueName + " handler");

            _queueService.PurgeQueues(_messageExpiration);

            _logger.Info(
                $"Purge Interval for {_queueService.RequestQueueName}: {_purgeInterval.TotalHours} hrs," +
                $" {_purgeInterval.Minutes } mins," +
                $" {_purgeInterval.Seconds} secs"
                );
        }
    }
}