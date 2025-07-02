using LoggerHelper;
using MSMQHandlerService.Services;
using QueueService;
using System;
using System.ServiceProcess;
using static QueueService.QueueServiceBase;

namespace MSMQHandlerService
{
    partial class MSMQImanageHandler : ServiceBase
    {
        ILogger _logger;
        QueueServiceHandlerProcessQueues _handler;
        QueueServiceHandlerPurgeQueues _purgeHandler;

        public MSMQImanageHandler()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "MSMQImanageHandler",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogDirectory,
                            Properties.Settings.Default.LogSizeMB,
                            Properties.Settings.Default.LogFileName),
                        new NlogEventTargetConfiguration(Properties.Settings.Default.ApplicationEventId)
                    }));

            InitializeComponent();
            InitializeProcessQueueHandler();
            InitializePurgeHandler();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (_handler == null)
                    InitializeProcessQueueHandler();
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
                    _logger.Info("Imanage handler already stopped.");
                else
                    _handler.Stop();
                if (_purgeHandler == null)
                    _logger.Info("Imanage purge handler already stopped.");
                else
                    _purgeHandler.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void InitializeProcessQueueHandler()
        {
            try
            {
                ImanageService.SetDeclareAsRecordExtensions(Properties.Settings.Default.ImanageDeclareAsRecordExtensions);
                var imanageCacheTimeout = Properties.Settings.Default.ImanageCacheTimeout;
                _logger.Info($"ImanageCacheTimeout: {imanageCacheTimeout.TotalSeconds} seconds");

                //var startupTimeout = Properties.Settings.Default.ImanageQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.ImanageQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                var queueServiceImanage = new QueueServiceImanage(QueueType.MSMQ, true, _logger);
                var cacheService = new CacheService(_logger) { ConnectionTimeout = imanageCacheTimeout };
                _handler = new QueueServiceHandlerProcessQueues(
                    queueServiceImanage,
                    new ImanageService(
                        queueServiceImanage, 
                        new QueueServiceEws(QueueType.MSMQ, true, _logger),
                        cacheService,
                        new FileService(_logger), 
                        new EwsService(new QueueServiceEws(QueueType.MSMQ, true, _logger), cacheService, _logger),
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
                var messageExpiration = Properties.Settings.Default.ImanageQueueServiceMessageExpiration;
                _logger.Info($"MessageExpiration: {messageExpiration.TotalHours} hours");
                var purgeInterval = Properties.Settings.Default.ImanageQueueServicePurgeTime;
                _logger.Info($"PurgeInterval: {purgeInterval.TotalHours} hours");
                //var startupTimeout = Properties.Settings.Default.ImanageQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.ImanageQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                _purgeHandler = new QueueServiceHandlerPurgeQueues(
                    new QueueServiceImanage(QueueType.MSMQ, true, _logger),
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
