using ININ.ICWS.Interactions;
using System.Threading.Tasks;
using static ININ.ICWS.Interactions.InteractionsResource;

namespace PureConnect
{
    public class ININInteractionsResource : IININInteractionsResource
    {
        IININService _ininService;
        InteractionsResource _interactionsResource;

        public ININInteractionsResource(IININService ininService)
        {
            _ininService = ininService;
        }

        public void Connect(string server, string username, string password)
        {
            _interactionsResource = new InteractionsResource(_ininService.Connect(server, username, password));
        }

        public async Task<ININUpdateInteractionAttributesResponse> UpdateInteractionAttributesAsync(
            ININUpdateInteractionAttributesRequestParameters ininUpdateInteractionAttributesRequestParameters,
            ININInteractionAttributesUpdateDataContract ininInteractionAttributesUpdateDataContract
            )
        {
            return new ININUpdateInteractionAttributesResponse(
                await _interactionsResource.UpdateInteractionAttributes(
                    new UpdateInteractionAttributesRequestParameters()
                    {
                        InteractionId = ininUpdateInteractionAttributesRequestParameters.InteractionId
                    },
                    new InteractionAttributesUpdateDataContract()
                    {
                        Attributes = ininInteractionAttributesUpdateDataContract.Attributes
                    }));

        }

        public async Task<ININGetInteractionAttributesResponse> GetInteractionAttributesAsync(
            ININGetInteractionAttributesRequestParameters ininGetInteractionAttributesRequestParameters
            )
        {
            return new ININGetInteractionAttributesResponse(
                await _interactionsResource.GetInteractionAttributes(
                    new GetInteractionAttributesRequestParameters()
                    {
                        InteractionId = ininGetInteractionAttributesRequestParameters.InteractionId,
                        Select = ininGetInteractionAttributesRequestParameters.Select
                    }));
        }
    }
}
