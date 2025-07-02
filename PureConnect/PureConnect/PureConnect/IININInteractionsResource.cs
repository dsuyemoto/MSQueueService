using System.Threading.Tasks;

namespace PureConnect
{
    public interface IININInteractionsResource
    {
        void Connect(string server, string username, string password);
        Task<ININGetInteractionAttributesResponse> GetInteractionAttributesAsync(
            ININGetInteractionAttributesRequestParameters ininGetInteractionAttributesRequestParameters
            );
        Task<ININUpdateInteractionAttributesResponse> UpdateInteractionAttributesAsync(
            ININUpdateInteractionAttributesRequestParameters ininUpdateInteractionAttributesRequestParameters,
            ININInteractionAttributesUpdateDataContract ininInteractionAttributesUpdateDataContract
            );
    }
}