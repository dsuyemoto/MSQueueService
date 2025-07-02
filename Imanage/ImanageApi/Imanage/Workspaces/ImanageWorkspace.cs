namespace Imanage
{
    public class ImanageWorkspace
    {
        public string Category { get; set; }
        public string Database { get; set; }
        public string Description { get; set; }
        public int FolderID { get; set; }
        public string Name { get; set; }
        public string ObjectID { get; set; }
        public string[] SubFolders { get; set; }
        public string SubType { get; set; }
        public ImanageError Error { get; set; }

        public ImanageWorkspace(
            string category,
            string database,
            string description,
            int folderId,
            string name,
            string objectId, 
            string[] subfolders,
            string subtype)
        {
            Category = category;
            Database = database;
            Description = description;
            FolderID = folderId;
            Name = name;
            ObjectID = objectId;
            SubFolders = subfolders;
            SubType = subtype;
        }
    }
}
