using iManage.WorkSite.Web.ServicesProxy;
using System.Net;

namespace Imanage
{
    public class ImanageConnection
    {
        Connection _connection;

        public ImanageIwovServices ImanageIwovServices { get; }

        public ImanageConnection(string url, string authType, NetworkCredential networkCredential)
        {
            _connection = new Connection(url, authType, networkCredential);
            ImanageIwovServices = new ImanageIwovServices(_connection.IWOVServices);
        }
    }
}
