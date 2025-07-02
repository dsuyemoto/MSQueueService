using ININ.WebServices.Core;

namespace PureConnect
{
    public interface IININService
    {
        WebServiceUtility Connect(string server, string username, string password, int maxTries=3);
    }
}