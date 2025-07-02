namespace ServiceNow
{
    public class SnTicket : ServiceNowBase
    {
        public string SnUsername { get; set; }
        public string SnPassword { get; set; }

        protected SnTicket()
        {
        }

        public SnTicket(string instanceurl, string username, string password)
        {
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
        }

        protected SnTicket(string tablename, string instanceurl, string username, string password)
        {
            TableName = tablename;
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
        }

        public SnResultBase Create(IServiceNowRepository serviceNowRepository)
        {
            throw new System.NotImplementedException();
        }

        public SnResultBase Update(IServiceNowRepository serviceNowRepository)
        {
            throw new System.NotImplementedException();
        }
    }
}
