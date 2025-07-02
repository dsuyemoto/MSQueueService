using LoggerHelper;
using System.Collections.Generic;

namespace EWS
{
    public interface IEwsWrapper
    {
        ExchFolder GetFolder(string[] folderPath, string mailboxEmailAddress, int maxResults = 1000);
        ExchFolder GetFolder(string uniqueId);
        ExchFolder CreateFolder(ExchFolder parentFolder, string foldername);
        void DeleteFolder(ExchFolder exchangeFolder, bool moveToDeletedItems = true);
        List<ExchFolder> FindFolders(string parentFolderId, string folderName, int maxResults = 1000);
        ExchFolder GetWellKnownFolder(string folderName, string mailboxEmailAddress);
        IEnumerable<ExchEmail> GetEmails(ExchFolder exchangeFolder, int maxResults = 1000);
        IEnumerable<ExchEmail> GetEmails(ExchFolder exchangeFolder, ExchSearchFilter exchangeSearchFilter, int maxResults = 1000);
        ExchEmail GetEmail(string uniqueId);
    }
}