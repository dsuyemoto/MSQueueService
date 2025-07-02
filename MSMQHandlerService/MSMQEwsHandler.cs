using LoggerHelper;
using MSMQHandlerService.Services;
using QueueService;
using System;
using System.ServiceProcess;
using static QueueService.QueueServiceBase;

namespace MSMQHandlerService
{
    partial class MSMQEwsHandler : ServiceBase
    {
        QueueServiceHandlerProcessQueues _handler;
        QueueServiceHandlerPurgeQueues _purgeHandler;
        ILogger _logger;

        public MSMQEwsHandler()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "MSMQEwsHandler",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogDirectory,
                            Properties.Settings.Default.LogSizeMB,
                            Properties.Settings.Default.LogFileName),
                        new NlogEventTargetConfiguration(Properties.Settings.Default.ApplicationEventId)
                    }));

            InitializeComponent();
            InitializeHandler();
            InitializePurgeHandler();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (_handler == null)
                    InitializeHandler();
                if (_purgeHandler == null)
                    InitializePurgeHandler();

                _handler.Start();
                _purgeHandler.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (_handler == null)
                    _logger.Info("Ews handler already stopped.");
                else
                    _handler.Stop();
                if (_purgeHandler == null)
                    _logger.Info("Ews purge handler already stopped.");
                else
                    _purgeHandler.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void InitializeHandler()
        {
            try
            {
                EwsService.RetryMaxWaitTimeSeconds = Properties.Settings.Default.EwsDefaultMaxWaitTimeSeconds;
                EwsService.RetryIntervalMilliseconds = Properties.Settings.Default.EwsRetryIntervalMilliseconds;
                EwsService.EwsTraceEnabled = Properties.Settings.Default.EwsTraceEnabled;
                EwsService.EwsTraceFolderPath = Properties.Settings.Default.EwsTraceFolderPath;

                //var startupTimeout = Properties.Settings.Default.EwsQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.EwsQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                var queueService = new QueueServiceEws(QueueType.MSMQ, true, _logger);
                _handler = new QueueServiceHandlerProcessQueues(
                    queueService,
                    new EwsService(
                        queueService, 
                        new CacheService(_logger) { ConnectionTimeout = new TimeSpan(24,0,0) }, 
                        _logger).HandlerCallbackAsync,
                    _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void InitializePurgeHandler()
        {
            try
            {
                var messageExpiration = Properties.Settings.Default.EwsQueueServiceMessageExpiration;
                _logger.Info($"MessageExpiration: {messageExpiration.TotalHours} hours");
                var purgeInterval = Properties.Settings.Default.EwsQueueServicePurgeTime;
                _logger.Info($"PurgeInterval: {purgeInterval.TotalHours} hours");
                //var startupTimeout = Properties.Settings.Default.EwsQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.EwsQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                _purgeHandler = new QueueServiceHandlerPurgeQueues(
                    new QueueServiceEws(QueueType.MSMQ, true, _logger),
                    messageExpiration,
                    purgeInterval, 
                    _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
