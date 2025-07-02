using IManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImanageCOM
{
    public class ImanSession
    {
        IManSession _imanSession;

        public ImanSession(IImanDMSWrapper imanDmsWrapper, ISessionParameters sessionParameters)
        {
            _imanSession = imanDmsWrapper.Add(sessionParameters.SessionName);

            if (String.IsNullOrEmpty(sessionParameters.ServiceUsername) || String.IsNullOrEmpty(sessionParameters.ServicePassword))
            {
                _imanSession.TrustedLogin();
            }
            else
            {
                _imanSession.Login(sessionParameters.ServiceUsername, sessionParameters.ServicePassword);
            }
        }
    }
}
