using EWS;
using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class EwsFolderDTO
    {
        public string ParentFolderUniqueId { get; set; }
        public string UniqueId { get; set; }       
        public string Name { get; set; }

        public EwsFolderDTO()
        {

        }

        public EwsFolderDTO(ExchFolder exchFolder)
        {
            if (exchFolder != null)
            {
                ParentFolderUniqueId = exchFolder.ParentFolderUniqueId;
                UniqueId = exchFolder.UniqueId;
                Name = exchFolder.Name;
            }
        }

        public EwsFolderDTO(EwsFolderQueue ewsFolderQueue)
        {
            ParentFolderUniqueId = ewsFolderQueue.ParentFolderUniqueId;
            UniqueId = ewsFolderQueue.UniqueId;
            Name = ewsFolderQueue.Name;
        }
    }
}