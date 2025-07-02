using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System.Collections.Generic;
using System.Net;

namespace ServiceNow
{
    internal abstract class ServiceNowRequest
    {
        protected IRestClient _restClient;

        protected Method _method;
        protected string _requestUrl;
        protected byte[] _contentBytes;
        protected string _fileName;
        protected string _mimeType;
        protected string _instanceUrl;
        protected Dictionary<string, string> _parameters;
        protected string _tableName;
        protected string[] _resultNames;
        protected Dictionary<string, string> _body;

        protected const string SYSPARMEXCLUDEREFERENCELINK = "sysparm_exclude_reference_link";
        protected const string SYSPARMFIELDS = "sysparm_fields";
        protected const string SYSUSER = "sys_user";
        protected const string SYSUSERGROUP = "sys_user_group";
        protected const string ATTACHMENT = "attachment";
        protected const string SYSPARMLIMIT = "sysparm_limit";

        public ServiceNowRequest(IRestClient restClient, string instanceUrl, string tableName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _restClient = restClient;
            _instanceUrl = instanceUrl;
            _tableName = tableName;
            _parameters = new Dictionary<string, string>();
            _body = new Dictionary<string, string>();
        }

        public SnResultBase Execute() 
        {
            return GetResponse(_restClient.Execute(GetRequest()));
        }

        protected RestRequest GetRequest()
        {
            var request = new RestRequest(_requestUrl, _method);
            request.JsonSerializer = new JsonNetSerializer();
            if (_body.Count > 0)
                request.AddJsonBody(_body);
            if (_resultNames != null && _resultNames.Length > 0)
                request.AddParameter(SYSPARMFIELDS, string.Join(",", _resultNames));
            if (_parameters.Count > 0)
                foreach (var parameter in _parameters)
                    request.AddParameter(parameter.Key, parameter.Value);            
            if (_contentBytes != null)
                request.AddFileBytes("uploadFile", _contentBytes, _fileName, _mimeType);

            return request;
        }

        public abstract SnResultBase GetResponse(IRestResponse response);
    }
}
