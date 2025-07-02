using System.Collections.Generic;

namespace ServiceNow
{
    public class SnTicketGet : SnTicket
    {
        public SnTicketGet(
            string tablename,
            string instanceurl, 
            string username,
            string password,
            Dictionary<string, string> snFields, 
            string[] resultnames,
            string sysId = "")
            : base(tablename, instanceurl, username, password)
        {
            ResultNames = resultnames;
            SnFields = snFields;
            SysId = sysId;
        }
    }
}
