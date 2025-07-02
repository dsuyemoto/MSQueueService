using Microsoft.Exchange.WebServices.Data;

namespace EWS
{
    public class ExchFolder
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string ParentFolderUniqueId { get; set; }

        public ExchFolder() { }

        public ExchFolder(Folder folder)
        {
            folder.Load();
            Name = folder.DisplayName;
            UniqueId = folder.Id.UniqueId;
            ParentFolderUniqueId = folder.ParentFolderId.UniqueId;
        }
    }
}
