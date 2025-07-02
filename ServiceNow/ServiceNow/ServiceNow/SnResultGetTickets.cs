using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using static ServiceNow.ServiceNowObjectBase;

namespace ServiceNow
{
    public class SnResultGetTickets : SnResultBase
    {
        public SnResultGetTickets(IRestResponse restResponse)
        {
            var responseGetTicketDTO = JsonConvert.DeserializeObject<SnResponseGetTicketsDTO>(restResponse.Content);
            if (responseGetTicketDTO != null)
            {
                if (responseGetTicketDTO.Result != null)
                {
                    foreach (var result in responseGetTicketDTO.Result)
                    {
                        var snFields = new Dictionary<string, string>();
                        snFields.Add(SnField.sys_id.ToString(), result.sys_id);
                        snFields.Add(SnField.number.ToString(), result.number);
                        snFields.Add(SnField.assignment_group.ToString(), result.assignment_group);
                        SnFieldsList.Add(snFields);
                    }
                }
                Error = responseGetTicketDTO.Error;
            }
            StatusCode = restResponse.StatusCode.ToString();
            IsSuccessful = false;
            if (restResponse.StatusCode == HttpStatusCode.OK)
                IsSuccessful = true;
        }
    }
}