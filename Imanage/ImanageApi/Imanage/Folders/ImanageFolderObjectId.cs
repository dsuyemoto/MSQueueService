using System;
using System.Text.RegularExpressions;

namespace Imanage
{
    public class ImanageFolderObjectId : ImanageObjectId
    {
        public string FolderId { get; set; }

        public ImanageFolderObjectId(string session, string database, string folderid)
        {
            if (string.IsNullOrEmpty(session)) throw new Exception("Session is empty or null");
            Session = session;
            if (string.IsNullOrEmpty(database)) throw new Exception("Database is empty or null");
            Database = database;
            if (string.IsNullOrEmpty(folderid)) throw new Exception("Folderid is empty or null");
            FolderId = folderid;
        }

        public ImanageFolderObjectId(string folderObjectId)
        {
            var matches = Regex.Matches(folderObjectId, "!session:(.*):!database:(.*):!folder:ordinary,(.*):");
            if (matches.Count > 0)
            {
                Session = matches[0].Groups[1].Value;
                Database = matches[0].Groups[2].Value;
                FolderId = matches[0].Groups[3].Value;
            }
        }

        public override string GetObjectId()
        {
            return string.Format("!nrtdms:0:!session:{0}:!database:{1}:!folder:ordinary,{2}:",
                    Session, Database, FolderId);
        }
    }
}
