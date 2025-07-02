namespace ServiceNow
{
    public class SnGroup : ServiceNowBase
    {
        public string SnUsername { get; set; }
        public string SnPassword { get; set; }

        protected SnGroup(string instanceurl, string username, string password)
        {
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
        }
    }
}
