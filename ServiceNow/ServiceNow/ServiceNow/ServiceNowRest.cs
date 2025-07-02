using RestSharp;
using RestSharp.Authenticators;

namespace ServiceNow
{
    public class ServiceNowRest : IServiceNowRest
    {
        IRestClient _restClient;

        const string SYSPARMFIELDS = "sysparm_fields";
        public const string SYSPARMEXCLUDEREFERENCELINK = "sysparm_exclude_reference_link";
        const string UCOMMUNICATIONS = "u_communications";
        const string UFINANCEINCIDENT = "u_finance_incident";

        public enum RestAction
        {
            table,
            attachment,
            sys_journal_field
        }

        public ServiceNowRest(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public ServiceNowRest()
        {
            
        }

        private IRestClient GetClient(string username, string password, string instanceUrl)
        {
            if (_restClient == null)
            {
                var restClient = new RestClient($"{instanceUrl}api/now");
                restClient.Authenticator = new HttpBasicAuthenticator(username, password);
                return restClient;
            }

            return _restClient;
        }

        public SnResultBase CreateTicket(SnTicketCreate snTicketCreate)
        {
            var request = new ServiceNowRequestTicketPost(
                snTicketCreate.TableName,
                snTicketCreate.SnFields,
                GetClient(snTicketCreate.SnUsername, snTicketCreate.SnPassword, snTicketCreate.InstanceUrl),
                snTicketCreate.InstanceUrl);

            return request.Execute();
        }

        public SnResultBase UpdateTicket(SnTicketUpdate snTicketUpdate)
        {
            var request = new ServiceNowRequestTicketPut(
                snTicketUpdate.TableName,
                snTicketUpdate.SysId,
                snTicketUpdate.SnFields,
                GetClient(snTicketUpdate.SnUsername, snTicketUpdate.SnPassword, snTicketUpdate.InstanceUrl),
                snTicketUpdate.InstanceUrl);

            var response = request.Execute();

            return response;
        }

        public SnResultBase GetTicket(SnTicketGet snTicketGet)
        {
            var request = new ServiceNowRequestTicketGet(
                snTicketGet.TableName,
                snTicketGet.SnFields,
                snTicketGet.ResultNames,
                GetClient(snTicketGet.SnUsername, snTicketGet.SnPassword, snTicketGet.InstanceUrl),
                snTicketGet.InstanceUrl,
                snTicketGet.SysId);

            return request.Execute();
        }

        public SnResultBase DeleteTicket(SnTicketDelete snTicketDelete)
        {
            var request = new ServiceNowRequestTicketDelete(
                snTicketDelete.TableName,
                snTicketDelete.SysId,
                GetClient(snTicketDelete.SnUsername, snTicketDelete.SnPassword, snTicketDelete.InstanceUrl),
                snTicketDelete.InstanceUrl);

            return request.Execute();
        }

        public SnResultBase CreateAttachment(SnAttachmentCreate snAttachmentCreate)
        {
            var request = new ServiceNowRequestAttachmentPost(
                snAttachmentCreate.TableName,
                snAttachmentCreate.SysId,
                snAttachmentCreate.ContentBytes,
                snAttachmentCreate.FileName,
                snAttachmentCreate.MimeType,
                snAttachmentCreate.SnFields,
                GetClient(snAttachmentCreate.SnUsername, snAttachmentCreate.SnPassword, snAttachmentCreate.InstanceUrl),
                snAttachmentCreate.InstanceUrl);

            return request.Execute();
        }

        public SnResultBase DeleteAttachment(SnAttachmentDelete snAttachmentDelete)
        {
            var request = new ServiceNowRequestAttachmentDelete(
                snAttachmentDelete.SysId,
                GetClient(snAttachmentDelete.SnUsername, snAttachmentDelete.SnPassword, snAttachmentDelete.InstanceUrl),
                snAttachmentDelete.InstanceUrl,
                snAttachmentDelete.TableName);

            return request.Execute();
        }

        public SnResultBase GetUser(SnUserGet snUserGet)
        {
            var request = new ServiceNowRequestUserGet(
                snUserGet.SnFields,
                snUserGet.ResultNames,
                GetClient(snUserGet.SnUsername, snUserGet.SnPassword, snUserGet.InstanceUrl),
                snUserGet.InstanceUrl,
                snUserGet.TableName,
                snUserGet.SysId);

            return request.Execute();
        }

        public SnResultBase GetGroup(SnGroupGet snGroupGet)
        {
            var request = new ServiceNowRequestGroupGet(
                snGroupGet.SysId,
                snGroupGet.SnFields,
                snGroupGet.ResultNames,
                GetClient(snGroupGet.SnUsername, snGroupGet.SnPassword, snGroupGet.InstanceUrl),
                snGroupGet.InstanceUrl,
                snGroupGet.TableName);

            return request.Execute();
        }
    }
}
