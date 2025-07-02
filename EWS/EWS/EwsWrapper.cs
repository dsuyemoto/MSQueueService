using LoggerHelper;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Net;

namespace EWS
{
    public partial class EwsWrapper : IEwsWrapper
    {
        ILogger _logger;
        ExchangeService _exchangeService;

        public EwsWrapper(EwsConnect ewsConnect, ILogger logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;
            _logger.Debug(ewsConnect);

            _exchangeService = new ExchangeService(ExchangeVersion.Exchange2013);
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _exchangeService.UseDefaultCredentials = true;

            if (ewsConnect.ServiceAccountCredentials != null)
                _exchangeService.Credentials = ewsConnect.ServiceAccountCredentials;
           
            _logger.Debug("Requested Exchange Version: " + _exchangeService.RequestedServerVersion);
            _logger.Debug("Server Info: " + _exchangeService.ServerInfo);

            if (ewsConnect.IsTraceEnabled)
            {
                _exchangeService.TraceListener = TraceListener.StartNewLog(ewsConnect.TraceFolderPath, new FileWrapper(), _logger);
                _exchangeService.TraceEnabled = true;
                _logger.Debug("Trace enabled");
            }

            AutodiscoverUrl(ewsConnect.AutodiscoverEmailAddress);
        }

        //https://msdn.microsoft.com/en-us/library/office/dd633677(v=exchg.80).aspx
        private static bool CertificateValidationCallBack(object sender,
                System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                return true;

            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject != certificate.Issuer) ||
                           (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                                return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        //https://msdn.microsoft.com/en-us/library/office/jj220499(v=exchg.80).aspx
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            bool result = false;
            var redirectionUri = new Uri(redirectionUrl);
            if (redirectionUri.Scheme == "https")
                result = true;

            return result;
        }

        private void AutodiscoverUrl(string mailboxEmailAddress)
        {
            try
            {
                _exchangeService.AutodiscoverUrl(mailboxEmailAddress, RedirectionUrlValidationCallback);
                _logger.Debug("AutodiscoverUrl: " + _exchangeService.Url);
            }
            catch (Microsoft.Exchange.WebServices.Autodiscover.AutodiscoverRemoteException aex)
            {
                _logger.Error(aex);
                throw new AutodiscoverLocalException(aex.Error.Message);
            }

        }
    }
}
