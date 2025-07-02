using System.Collections.Generic;
using System.Net;
using static ININ.ICWS.Interactions.InteractionsResource;

namespace PureConnect
{
    public class ININGetInteractionAttributesResponse
    {
        public bool IsSuccessful { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ININInteractionAttributesDataContract InteractionAttributesDataContract { get; set; }

        public ININGetInteractionAttributesResponse()
        {

        }

        public ININGetInteractionAttributesResponse(GetInteractionAttributesResponses getInteractionAttributesResponses)
        {
            IsSuccessful = false;

            if (getInteractionAttributesResponses != null)
            {
                StatusCode = getInteractionAttributesResponses.StatusCode;

                getInteractionAttributesResponses.PerformIfResponseIs200((dataContract) =>
                {
                    IsSuccessful = true;
                    InteractionAttributesDataContract = new ININInteractionAttributesDataContract(dataContract);
                });
            }
        }
    }
}
