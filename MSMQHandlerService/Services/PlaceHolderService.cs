using ServiceNow;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ServiceNow.ServiceNowBase;

namespace MSMQHandlerService
{
    public class PlaceHolderService
    {
        IServiceNowRepository _serviceNowRepository;
        string _instanceUrl;
        string _username;
        string _passwordBase64;
        string _attachmentSysId;
        int _timerSeconds;

        public PlaceHolderService(IServiceNowRepository serviceNowRepository, int timerSeconds = 10)
        {
            _serviceNowRepository = serviceNowRepository;
            _timerSeconds = timerSeconds;
        }

        public async Task<SnResultBase> Create(
            string tableName,
            string instanceUrl,
            string username,
            string passwordBase64,
            string mimeType,
            string ticketSysId,
            string placeHolderContent, 
            string placeHolderName,
            CancellationToken token)
        {
            _instanceUrl = instanceUrl;
            _username = username;
            _passwordBase64 = passwordBase64;
            
            var content = Encoding.UTF8.GetBytes(placeHolderContent);

            return await Task.Run(() => {
                var result = _serviceNowRepository.CreateAttachment(
                    new SnAttachmentCreate(
                        tableName,
                        instanceUrl,
                        username,
                        Helpers.Base64ConvertFrom(passwordBase64),
                        placeHolderName + ".txt",
                        mimeType,
                        ticketSysId,
                        content));

                var snFields = ((SnResultTable)result).SnFields;
                if (snFields.ContainsKey(SnField.sys_id.ToString()))
                    _attachmentSysId = (string)snFields[SnField.sys_id.ToString()];

                return result;
                    }, token);
        }

        public async Task<SnResultBase> Delete(CancellationToken token)
        {
            return await Task.Run(() => {
                var timer = _timerSeconds;
                while (string.IsNullOrEmpty(_attachmentSysId) ) 
                {
                    if (timer < 0) return null;
                    Thread.Sleep(1000);
                    timer--;
                }

                return _serviceNowRepository.DeleteAttachment(
                    new SnAttachmentDelete(
                        _instanceUrl,
                        _username,
                        Helpers.Base64ConvertFrom(_passwordBase64),
                        _attachmentSysId
                    ));
                }, token);
        }
    }
}
