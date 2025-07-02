using System.Collections.Generic;

namespace ServiceNow
{
    public class SnObjectGet : ServiceNowBase
    {
        public string SnUsername { get; set; }
        public string SnPassword { get; set; }

        public SnObjectGet(string tablename, string instanceurl, string username, string password, Dictionary<string, string> fields, string[] resultnames)
        {
            TableName = tablename;
            InstanceUrl = instanceurl;
            SnUsername = username;
            SnPassword = password;
            SnFields = fields;
            ResultNames = resultnames;
        }
    }
}
