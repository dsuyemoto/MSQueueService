using EWS;

namespace MSMQHandlerService.Models
{
    public class EwsFolderQueue
    {
        public string ParentFolderUniqueId { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }

        public EwsFolderQueue()
        {

        }

        public EwsFolderQueue(ExchFolder exchFolder)
        {
            if (exchFolder != null)
            {
                ParentFolderUniqueId = exchFolder.ParentFolderUniqueId;
                UniqueId = exchFolder.UniqueId;
                Name = exchFolder.Name;
            }
        }
    }
}