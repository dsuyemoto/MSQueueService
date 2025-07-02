using System.Collections.Generic;
using static ServiceNow.ServiceNowObjectBase;

namespace ServiceNow
{
    public interface IServiceNowObject
    {
        Dictionary<string, string> SnFields { get; }
        string SysId { get; }
        string TableName { get; set; }
        string[] ResultNames { get; }
        SoapAction ?Action { get; }
        string InstanceUrl { get;  }
        string SnUsername { get; set; }
        string SnPassword { get; set; }
    }
}
