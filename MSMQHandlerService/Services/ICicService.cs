using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface ICicService
    {
        Task<object> HandlerCallbackAsync(object interaction, CancellationToken token);
    }
}