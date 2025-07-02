using ININ.ICWS.Configuration.System;
using System.Threading.Tasks;
using static ININ.ICWS.Configuration.System.SystemParametersResource;

namespace PureConnect
{
    public class ININSystemParametersResource : IININSystemParametersResource
    {

        public ININSystemParametersResource()
        {

        }

        private SystemParametersResource Connect(ININServerCredentials ininServerCredentials)
        {
            var ininService = new ININService();
            return new SystemParametersResource(ininService.Connect(ininServerCredentials));
        }

        public async Task<ININGetSystemParameterResponses> GetSystemParameterAsync(
            ININGetSystemParameterRequestParameters ininGetSystemParameterRequestParameters,
            ININServerCredentials ininServerCredentials)
        {
            var systemParametersResource = Connect(ininServerCredentials);

            return new ININGetSystemParameterResponses(
                await systemParametersResource.GetSystemParameter(
                    new GetSystemParameterRequestParameters()
                    {
                        Id = ininGetSystemParameterRequestParameters.Id,
                        RightsFilter = ininGetSystemParameterRequestParameters.RightsFilter,
                        Select = ininGetSystemParameterRequestParameters.Select
                    })
                );
        }
    }
}
