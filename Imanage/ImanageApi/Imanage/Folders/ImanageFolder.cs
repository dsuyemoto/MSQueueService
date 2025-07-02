using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imanage
{
    public class ImanageFolder
    {
        public string Description { get; set; }
        public bool IsRootLevelFolder { get; set; }
        public string Name { get; set; }
        public string ObjectID { get; set; }
        public string ParentID { get; set; }
        public ImanageError Error { get; set; }

        public ImanageFolder()
        {

        }

        public ImanageFolder(DocumentFolder documentFolder, ImanageError error)
        {
            Description = documentFolder.Description;
            IsRootLevelFolder = documentFolder.IsRootLevelFolder;
            Name = documentFolder.Name;
            ObjectID = documentFolder.ObjectID;
            ParentID = documentFolder.ParentID;
            Error = error;
        }
    }
}
