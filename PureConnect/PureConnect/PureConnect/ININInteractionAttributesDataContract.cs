using ININ.ICWS.Interactions;
using System.Collections.Generic;

namespace PureConnect
{
    public class ININInteractionAttributesDataContract
    {
        public Dictionary<string, string> Attributes { get; set; }

        public ININInteractionAttributesDataContract()
        {

        }

        public ININInteractionAttributesDataContract(InteractionAttributesDataContract interactionAttributesDataContract)
        {
            if (interactionAttributesDataContract != null)
                Attributes = interactionAttributesDataContract.Attributes;
        }
    }
}
