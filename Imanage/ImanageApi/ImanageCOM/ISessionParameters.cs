using System;
namespace ImanageCOM
{
    public interface ISessionParameters
    {
        string ServerName { get; set; }
        string SessionName { get; set; }
        string ServiceUsername { get; set; }
        string ServicePassword { get; set; }
    }
}
