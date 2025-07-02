using LoggerHelper;
using MSMQHandlerService.Services;
using QueueService;
using System;
using System.ServiceProcess;
using static QueueService.QueueServiceBase;

namespace MSMQHandlerService
{
    partial class MSMQCicHandler : ServiceBase
    {
        QueueServiceHandlerProcessQueues _handler;
        QueueServiceHandlerPurgeQueues _purgeHandler;
        ILogger _logger;

        public MSMQCicHandler()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "MSMQCicHandler",
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
                    _logger.Info("Cic handler already stopped.");
                else
                    _handler.Stop();
                if (_purgeHandler == null)
                    _logger.Info("Cic purge handler already stopped.");
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
                //var startupTimeout = Properties.Settings.Default.CicQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.CicQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                _handler = new QueueServiceHandlerProcessQueues(
                    new QueueServiceCic(QueueType.MSMQ, true, _logger),
                    new CicService(_logger).HandlerCallbackAsync,
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
                var purgeInterval = Properties.Settings.Default.CicQueueServicePurgeTime;
                _logger.Info($"PurgeInterval: {purgeInterval.TotalHours} hours");
                var messageExpiration = Properties.Settings.Default.CicQueueServiceMessageExpiration;
                _logger.Info($"MessageExpiration: {messageExpiration.TotalHours} hours");
                //var startupTimeout = Properties.Settings.Default.CicQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.CicQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                _purgeHandler = new QueueServiceHandlerPurgeQueues(
                    new QueueServiceCic(QueueType.MSMQ, true, _logger),
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
