using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace ServiceNow
{
    public class SnResultTable : SnResultBase
    {
        public Dictionary<string, object> SnFields { get; set; }

        public SnResultTable(IRestResponse restResponse, string instanceUrl, string tableName) 
            : base(restResponse, instanceUrl, tableName)
        {

        }

        protected override void GetSnFields(IRestResponse restResponse)
        {
            var responseTableDTO = JsonConvert.DeserializeObject<SnResponseTableDTO>(restResponse.Content);

            if (responseTableDTO != null)
            {
                SnFields = responseTableDTO.Result;
                Error = new SnErrorResult(responseTableDTO.Error);
                Status = responseTableDTO.Status;
            }
        }
    }
}
