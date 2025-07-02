using System.Text.RegularExpressions;

namespace Imanage
{
    public class ImanageDocumentObjectId : ImanageObjectId
    {
        public string Number { get; set; }
        public string Version { get; set; }

        public ImanageDocumentObjectId(string session, string database, string number, string version)
        {
            Session = session;
            Database = database;
            Number = number;
            Version = version;
        }

        public ImanageDocumentObjectId(string documentObjectId)
        {
            if (documentObjectId == null)
                return;
            var matches = Regex.Matches(documentObjectId, "!session:(.*):!database:(.*):!document:(.*),(.*):");
            if (matches.Count > 0)
            {
                Session = matches[0].Groups[1].Value;
                Database = matches[0].Groups[2].Value;
                Number = matches[0].Groups[3].Value;
                Version = matches[0].Groups[4].Value;
            }
        }
        public override string GetObjectId()
        {
            return string.Format("!nrtdms:0:!session:{0}:!database:{1}:!document:{2},{3}:",
                    Session, Database, Number, Version);
        }

    }
}
