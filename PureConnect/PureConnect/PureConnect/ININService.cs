using ININ.ICWS;
using ININ.ICWS.Common;
using ININ.ICWS.Connection;
using ININ.WebServices.Core;
using System;
using System.Collections.Generic;
using static ININ.ICWS.Connection.ConnectionResource;

namespace PureConnect
{
    public class ININService : IININService
    {
        private WebServiceUtility _webServiceUtility;
        private CreateConnectionRequestParameters _createConnectionRequestParameters;
        private IcAuthConnectionRequestSettingsDataContract _icAuthConnectionRequestSettingsDataContract;
        private List<string> _connectAlternateHosts = new List<string>();
        private bool _tryingBackupServer = false;

        public bool IsConnected { get; private set; } = false;

        public ININService()
        {
            _createConnectionRequestParameters = new CreateConnectionRequestParameters()
            {
                Accept_Language = "en-us",
                Include = "features"
            };
            _icAuthConnectionRequestSettingsDataContract = new IcAuthConnectionRequestSettingsDataContract()
            {
                ApplicationName = "PureConnectService",
            };
        }

        public WebServiceUtility Connect(ININServerCredentials ininServerCredentials)
        {
            return Connect(ininServerCredentials.Server, ininServerCredentials.Username, ininServerCredentials.Password);
        }

        public WebServiceUtility Connect(string server, string username, string password, int maxTries=3)
        {
            _icAuthConnectionRequestSettingsDataContract.UserID = username;
            _icAuthConnectionRequestSettingsDataContract.Password = password;

            ConnectServer(server);

            return _webServiceUtility;
        }

        private void ConnectServer(string server)
        {
            IsConnected = false;
            _webServiceUtility = new WebServiceUtility()
            {
                Port = 8018,
                IsHttps = false,
                Server = server
            };

            var result = new ConnectionResource(_webServiceUtility)
                .CreateConnection(
                    _createConnectionRequestParameters,
                    _icAuthConnectionRequestSettingsDataContract).Result;
            
            result.PerformIfResponseIs201(response => HandleConnection201(response, result.Set_Cookie));
            result.PerformIfResponseIs400(response => HandleConnectionError(response));
            result.PerformIfResponseIs410(response => HandleConnectionError(response));
            result.PerformIfResponseIs503(response => HandleConnection503(response));
        }

        private void HandleConnection201(ConnectionResponseDataContract response, string cookie)
        {
            _webServiceUtility.SessionParameters =
                new AuthenticationParameters
                {
                    SessionId = response.SessionId,
                    Cookie = cookie,
                    ININ_ICWS_CSRF_Token = response.CsrfToken
                };

            IsConnected = true;
            _connectAlternateHosts.Clear();
            _tryingBackupServer = false;
        }

        private void HandleConnectionError(ErrorDataContract errorDataContract)
        {
            throw new Exception(errorDataContract.Message);
        }

        private void HandleConnection503(AlternateHostsDataContract response)
        {
            if (response.AlternateHostListHasValue && !_tryingBackupServer)
            {
                _connectAlternateHosts = response.AlternateHostList;
                _tryingBackupServer = true;
            }
            if (!IsConnected && _connectAlternateHosts.Count > 0)
            {
                var alternateHost = _connectAlternateHosts[0];
                _connectAlternateHosts.Remove(alternateHost);
                ConnectServer(alternateHost);
            }
        }
    }
}
