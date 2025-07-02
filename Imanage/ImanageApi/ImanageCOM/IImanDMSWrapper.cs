using System;
using IManage;

namespace ImanageCOM
{
    public interface IImanDMSWrapper
    {
        IManSession Add(string sessionName);
    }
}
