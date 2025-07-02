using LoggerHelper;
using MSMQHandlerService.Services;
using QueueService;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using static QueueService.QueueServiceBase;

namespace QueueServiceWebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

#if ENABLEHANDLERS
            var logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "QueueServiceWebApp",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogFilePath,
                            Properties.Settings.Default.LogFileSizeMB,
                            "Application_Start") }
                    ));

            StartHandler(
                new QueueServiceImanage(QueueType.MSMQ, true, logger),
                new ImanageService(
                    new QueueServiceImanage(QueueType.MSMQ, true, logger),
                    new QueueServiceEws(QueueType.MSMQ, true, logger),
                    new CacheService(logger),
                    new FileService(logger),
                    new EwsService(new QueueServiceEws(QueueType.MSMQ, true, logger), new CacheService(logger), logger),
                    logger).HandlerCallbackAsync,
                logger);
            StartHandler(
                new QueueServiceCic(QueueType.MSMQ, true, logger),
                new CicService(logger).HandlerCallbackAsync,
                logger);
            StartHandler(
                new QueueServiceServiceNow(QueueType.MSMQ, true, logger), 
                new ServiceNowService(logger).HandlerCallbackAsync,
                logger);
            StartHandler(
                new QueueServiceEws(QueueType.MSMQ, true, logger),
                new EwsService(
                    new QueueServiceEws(QueueType.MSMQ, true, logger), 
                    new CacheService(logger), 
                    logger).HandlerCallbackAsync,
                logger);
#endif
        }
#if ENABLEHANDLERS
        private void StartHandler(
            IQueueService queueService,
            Func<object, CancellationToken, Task<object>> callback,
            ILogger logger)
        {
            try
            {
                var handler = new QueueServiceHandlerProcessQueues(queueService, callback, logger);
                handler.Start();
                var purgeHandler = new QueueServiceHandlerPurgeQueues(
                    queueService,
                    new TimeSpan(3,0,0,0), 
                    new TimeSpan(3,0,0,0), 
                    logger);
                purgeHandler.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
#endif
    }
}
