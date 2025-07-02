using System.Net;
using static ININ.ICWS.Interactions.InteractionsResource;

namespace PureConnect
{
    public class ININUpdateInteractionAttributesResponse
    {
        public string InteractionId { get; set; }
        public bool IsSuccessful { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ININUpdateInteractionAttributesResponse()
        {

        }

        public ININUpdateInteractionAttributesResponse(UpdateInteractionAttributesResponses updateInteractionAttributesResponses)
        {
            updateInteractionAttributesResponses.PerformIfResponseIs200(() => { IsSuccessful = true; });
            StatusCode = updateInteractionAttributesResponses.StatusCode;
        }

    }
}
