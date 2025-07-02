using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImanageCOM
{
    public class SessionParameters : ISessionParameters
    {
        public string SessionName { get; set; }
        public string ServerName { get; set; }
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }

        public SessionParameters(string sessionName, string serverName, string serviceUsername, string servicePassword) 
        {
            SessionName = sessionName;
            ServerName = serverName;
            ServiceUsername = serviceUsername;
            ServicePassword = servicePassword;
        }
    }
}
