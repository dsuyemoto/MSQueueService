using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Imanage.DocumentProfileItems;

namespace MSMQHandlerService.Services
{
    public interface IImanageService
    {
        bool EnableDeclareAsRecord { get; set; }
        List<ProfileApplicationType> DeclareAsRecordExtensions { get; set; }
        Task<object> HandlerCallbackAsync(object document, CancellationToken token);
    }
}