namespace ServiceNow
{
    public class SnCommunicationsPost : ServiceNowBase
    {
        public string SnUsername { get; set; }
        public string SnPassword { get; set; }
        public string FieldValue { get; set; }

        public SnCommunicationsPost(
            string instanceurl,
            string username,
            string password,
            string tableSysId,
            string tableName,
            string fieldValue)
        {
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
            SysId = tableSysId;
            TableName = tableName;
            FieldValue = fieldValue;
        }
    }
}
