using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class EwsResultFolderDTO
    {
        public EwsFolderDTO Folder { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }

        public EwsResultFolderDTO()
        {

        }

        public EwsResultFolderDTO(EwsResultFolderQueue ewsResultFolderQueue)
        {
            Folder = new EwsFolderDTO(ewsResultFolderQueue.Folder);
            if (ewsResultFolderQueue.ErrorResult != null)
                ErrorResult = new ErrorResultDTO(ewsResultFolderQueue.ErrorResult);
        }
    }
}