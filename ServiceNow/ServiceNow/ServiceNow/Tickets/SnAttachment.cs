namespace ServiceNow
{
    public abstract class SnAttachment : ServiceNowBase
    {
        public string SnUsername { get; set; }
        public string SnPassword { get; set; }

        public SnAttachment(string instanceurl, string username, string password)
        {
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
        }
    }
}
