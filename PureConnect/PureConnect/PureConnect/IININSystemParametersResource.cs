using ININ.WebServices.Core;
using System.Threading.Tasks;

namespace PureConnect
{
    public interface IININSystemParametersResource
    {
        Task<ININGetSystemParameterResponses> GetSystemParameterAsync(
            ININGetSystemParameterRequestParameters ininGetSystemParameterRequestParameters,
            ININServerCredentials ininServerCredentials
            );
    }
}
