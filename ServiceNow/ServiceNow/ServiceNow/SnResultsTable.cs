using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace ServiceNow
{
    public class SnResultsTable : SnResultBase
    {
        public List<Dictionary<string, object>> SnFieldsList { get; set; }

        public SnResultsTable(IRestResponse restResponse, string instanceUrl, string tableName) 
            : base(restResponse, instanceUrl, tableName)
        {

        }

        protected override void GetSnFields(IRestResponse restResponse)
        {
            var responseTableDTO = JsonConvert.DeserializeObject<SnResponsesTableDTO>(restResponse.Content);

            if (responseTableDTO != null)
            {
                SnFieldsList = responseTableDTO.Result;
                Error = new SnErrorResult(responseTableDTO.Error);
                Status = responseTableDTO.Status;
            }
        }
    }
}
