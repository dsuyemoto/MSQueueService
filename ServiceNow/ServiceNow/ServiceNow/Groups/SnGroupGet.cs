using System.Collections.Generic;

namespace ServiceNow
{
    public class SnGroupGet : SnGroup
    {
        public SnGroupGet(
            string instanceurl,
            string username,
            string password,
            Dictionary<string, string> snFields,
            string[] resultnames,
            string sysId = "") : base(instanceurl, username, password)
        {
            ResultNames = resultnames;
            SnFields = snFields;
            SysId = sysId;
        }
    }
}
