using System.Collections.Generic;

namespace ServiceNow
{
    public class SnResponsesTableDTO
    {
        public List<Dictionary<string, object>> Result { get; set; }
        public ErrorDTO Error { get; set; }
        public string Status { get; set; }

        public SnResponsesTableDTO()
        {

        }
    }
}
