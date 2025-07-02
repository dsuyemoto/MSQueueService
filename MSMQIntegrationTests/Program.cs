using System;

namespace MSMQ.IntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting test...");
            MSMQIntegrationTest.TestHandlers();
            //MSMQIntegrationTest.TestMessageRetrieval();

            Console.Read();
        }
    }
}
