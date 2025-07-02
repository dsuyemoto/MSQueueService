using System.Net;
using static ININ.ICWS.Configuration.System.SystemParametersResource;

namespace PureConnect
{
    public class ININGetSystemParameterResponses
    {
        public bool IsSuccessful { get; private set; }
        public HttpStatusCode StatusCode { get; }
        public ININSystemParameterDataContract DataContract { get; set; }

        public ININGetSystemParameterResponses()
        {

        }

        public ININGetSystemParameterResponses(GetSystemParameterResponses getSystemParameterResponses)
        {
            IsSuccessful = false;

            if (getSystemParameterResponses != null)
            {
                StatusCode = getSystemParameterResponses.StatusCode;

                getSystemParameterResponses.PerformIfResponseIs200((d) =>
                {
                    IsSuccessful = true;
                    DataContract = new ININSystemParameterDataContract(d);
                });
            }
        }
    }
}
