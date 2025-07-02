using RestSharp;

namespace Imanage
{
    public abstract class ImanageRestRequestBase
    {
        protected abstract RestClient GetClient();
        protected abstract RestRequest GetRequest();
    }
}
