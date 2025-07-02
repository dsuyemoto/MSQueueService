using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EWS.Tests
{
    public class EwsWrapperConnector
    {
        EwsWrapper _ewsWrapper;

        public EwsWrapperConnector()
        {
            _ewsWrapper = new EwsWrapper(ConfigurationManager.AppSettings["Mailbox"],
                new NetworkCredential(
                    ConfigurationManager.AppSettings["UserName"], 
                    ConfigurationManager.AppSettings["Password"]
                    ));
        }

        public EwsWrapper Instance() { return _ewsWrapper; }
    }
}
