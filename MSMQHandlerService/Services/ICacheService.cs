using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface ICacheService
    {
        TimeSpan ConnectionTimeout { get; set; }
        Task<object> GetConnectionAsync(object credentials, Func<object> initializer, CancellationToken token);
    }
}