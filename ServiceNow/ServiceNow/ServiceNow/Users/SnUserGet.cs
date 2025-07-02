using System.Collections.Generic;

namespace ServiceNow
{
    public class SnUserGet : SnUser
    {
        public SnUserGet(
            string instanceurl, 
            string username, 
            string password,
            Dictionary<string, string> snFields,
            string[] resultnames,
            string sysId = "")
            : base(instanceurl, username, password)
        {
            ResultNames = resultnames;
            SnFields = snFields;
            SysId = sysId;
        }
    }
}
