using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public interface IFileService
    {
        Task<byte[]> ReadAsync(string filePath, CancellationToken token);
        Task SaveAsync(byte[] content, string filePath, CancellationToken token);
        void Delete(string filePath);
        bool Exists(string filePath);
    }
}