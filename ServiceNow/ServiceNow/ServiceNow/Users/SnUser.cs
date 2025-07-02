namespace ServiceNow
{
    public class SnUser : ServiceNowBase
    {
        public string SnUsername { get; set; }
        public string SnPassword { get; set; }

        protected SnUser(string instanceurl, string username, string password) 
        {
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
        }
    }
}
