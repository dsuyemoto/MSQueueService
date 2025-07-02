using System;

namespace LoggerHelper.Integration.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    "INFO", 
                    "LoggerHelper.Integration.Tests", 
                    new NlogTargetConfigurationBase[] { 
                        new NlogFileTargetConfiguration("C:\\Temp\\LoggerHelper\\IntegrationTests", 10, "LoggerHelper.Integration.Tests") 
                    }));

            Console.Read();
            
        }
    }

    class ByteContent
    {
        public byte[] Content { get; set; } = new byte[1];
        public ByteContent()
        {

        }
    }

}
