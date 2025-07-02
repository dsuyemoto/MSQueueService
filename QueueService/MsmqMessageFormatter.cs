using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MSMQ
{
    public class MsmqMessageFormatter : IMessageFormatter
    {
        public bool CanRead(Message message)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public object Read(Message message)
        {
            throw new NotImplementedException();
        }

        public void Write(Message message, object obj)
        {
            throw new NotImplementedException();
        }
    }
}
