using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IManage;

namespace ImanageCOM
{
    public class ImanDMSWrapper : IDisposable, IImanDMSWrapper
    {
        private readonly ManDMS dms = new ManDMS();

        public ImanDMSWrapper() { }

        public IManSession Add(string sessionName)
        {
            return dms.Sessions.Add(sessionName);
        }

        public void Dispose()
        {
            if (dms != null)
                dms.Close();
        }
    }
}
