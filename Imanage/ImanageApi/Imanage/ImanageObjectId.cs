using System;
using System.Text.RegularExpressions;

namespace Imanage
{
    public abstract class ImanageObjectId : IImanageObjectId
    {
        public string Database { get; set; }
        public string Session { get; set; }

        public abstract string GetObjectId();
    }
}
