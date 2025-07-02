using System.Collections;
using System.Collections.Generic;

namespace Soap
{
    public interface ISoapCall
    {
        IEnumerable<Dictionary<string, string>> Call(string rootNode, Dictionary<string, string> nodeHash, string serviceUrl,
            string username, string password, string resultRootNodeName);
    }
}
