using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IServiceNowService
    {
        Task<object> HandlerCallbackAsync(object ticket, CancellationToken token);
    }
}