namespace ServiceNow
{
    public class SnAttachmentDelete : SnTicket
    {
        public SnAttachmentDelete(
            string instanceurl, 
            string username,
            string password, 
            string sysId) 
            : base(instanceurl, username, password)
        {
            ResultNames = new string[] { SnField.count.ToString() };

            SetSnFieldValue(SnField.sys_id.ToString(), sysId, SnFields);
        }
    }
}
