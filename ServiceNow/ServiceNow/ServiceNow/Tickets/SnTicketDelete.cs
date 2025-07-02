namespace ServiceNow
{
    public class SnTicketDelete : SnTicket
    {
        const string SN_GLOBAL_TICKET_DELETION = "5bd543244655676b00ef38c077828cc6";

        public SnTicketDelete(
            string tablename,
            string instanceurl,
            string username, 
            string password,
            string sysid) 
            : base(tablename, instanceurl, username, password)
        {
            SysId = sysid;
            ResultNames = new string[] { SnField.count.ToString() };
            SetSnFieldValue(
                        SnField.assignment_group.ToString(),
                        SN_GLOBAL_TICKET_DELETION,
                        SnFields);
        }
    }
}
