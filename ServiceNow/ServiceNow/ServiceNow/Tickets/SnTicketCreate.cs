using System.Collections.Generic;

namespace ServiceNow
{
    public class SnTicketCreate : SnTicket
    {
        public SnTicketCreate(
            string tablename,
            string instanceurl, 
            string username, 
            string password, 
            Dictionary<string, string> snfields) 
            : base(tablename, instanceurl, username, password)
        {
            ResultNames = new string[] { SnField.number.ToString(), SnField.sys_id.ToString() };
            foreach (var snfield in snfields)
                SetSnFieldValue(snfield.Key, snfield.Value, SnFields);
        }
    }
}
