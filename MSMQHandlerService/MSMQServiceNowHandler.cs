using LoggerHelper;
using MSMQHandlerService.Services;
using QueueService;
using System;
using System.ServiceProcess;
using static QueueService.QueueServiceBase;

namespace MSMQHandlerService
{
    public partial class MSMQServiceNowHandler : ServiceBase
    {
        ILogger _logger;
        QueueServiceHandlerProcessQueues _handler;
        QueueServiceHandlerPurgeQueues _purgeHandler;

        public MSMQServiceNowHandler()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "MSMQServiceNowHandler", 
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogDirectory, 
                            Properties.Settings.Default.LogSizeMB, 
                            Properties.Settings.Default.LogFileName), 
                        new NlogEventTargetConfiguration(Properties.Settings.Default.ApplicationEventId) 
                    }));

            if (!string.IsNullOrEmpty(Properties.Settings.Default.ServiceNowPlaceholderName))
                ServiceNowService.PlaceholderName = Properties.Settings.Default.ServiceNowPlaceholderName;

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
                    _logger.Info("ServiceNow handler already stopped.");
                else
                    _handler.Stop();
                if (_purgeHandler == null)
                    _logger.Info("ServiceNow purge handler already stopped.");
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
                //var startupTimeout = Properties.Settings.Default.ServiceNowQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.ServiceNowQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                _handler = new QueueServiceHandlerProcessQueues(
                    new QueueServiceServiceNow(QueueType.MSMQ, true, _logger),
                    new ServiceNowService(_logger).HandlerCallbackAsync,
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
                var messageExpiration = Properties.Settings.Default.ServiceNowQueueServiceMessageExpiration;
                _logger.Info($"MessageExpiration: {messageExpiration.TotalHours} hours");
                var purgeInterval = Properties.Settings.Default.ServiceNowQueueServicePurgeTime;
                _logger.Info($"PurgeInterval: {purgeInterval.TotalHours} hours");

                //var startupTimeout = Properties.Settings.Default.ServiceNowQueueServiceStartupTimeout;
                //_logger.Info($"StartupTimeout: {startupTimeout.TotalSeconds} seconds");
                //var shutdownTimeout = Properties.Settings.Default.ServiceNowQueueServiceShutdownTimeout;
                //_logger.Info($"ShutdownTimeout: {shutdownTimeout.TotalSeconds} seconds");

                _purgeHandler = new QueueServiceHandlerPurgeQueues(
                    new QueueServiceServiceNow(QueueType.MSMQ, true, _logger), 
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
