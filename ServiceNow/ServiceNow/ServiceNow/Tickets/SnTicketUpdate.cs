using System.Collections.Generic;

namespace ServiceNow
{
    public class SnTicketUpdate : SnTicket
    {
        public SnTicketUpdate(string tablename, string instanceurl, string username, string password, string sysid, Dictionary<string, string> snfields) 
            : base(tablename, instanceurl, username, password)
        {
            SysId = sysid;
            ResultNames = new string[] { SnField.sys_id.ToString() };
            foreach (var snfield in snfields)
                SetSnFieldValue(snfield.Key, snfield.Value, SnFields);
        }
    }
}
