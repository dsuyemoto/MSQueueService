using LoggerHelper;
using System.ServiceProcess;

namespace MSMQHandlerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MSMQServiceNowHandler(),
                new MSMQCicHandler(),
                new MSMQEwsHandler(),
                new MSMQImanageHandler()
            };
            ServiceBase.Run(ServicesToRun);           
        }
    }
}
