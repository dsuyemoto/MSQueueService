using Imanage;

namespace ImportToImanageApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceUrl = args[0]; 
            var serviceUser = args[1];
            var servicePassword = args[2];

            var imanCreds = new ImanageCreds("database", serviceUser, servicePassword);

            var imanageService = new ImanageConnection(serviceUrl, authType, credentials);


        }
    }
}
