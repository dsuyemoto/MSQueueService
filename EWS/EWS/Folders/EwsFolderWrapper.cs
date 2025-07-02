using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;

namespace EWS
{
    public partial class EwsWrapper
    {
        public ExchFolder GetFolder(string[] folderPath, string mailboxEmailAddress, int maxResults = 1000)
        {
            var msgRootExchFolder = new ExchFolder(
                Folder.Bind(
                    _exchangeService,
                    new FolderId(
                        WellKnownFolderName.MsgFolderRoot,
                        mailboxEmailAddress)));

            var parentFolderId = msgRootExchFolder.UniqueId;
            ExchFolder exchFolder = null;
            foreach (var folder in folderPath)
            {
                _logger.Debug("folder: " + folder);

                if (!string.IsNullOrEmpty(folder))
                {
                    var exchFolders = FindFolders(parentFolderId, folder, maxResults) as List<ExchFolder>;
                    if (exchFolders.Count == 0) return null;

                    exchFolder = exchFolders[0];
                    _logger.Debug(exchFolder);
                    parentFolderId = exchFolder.UniqueId;
                    _logger.Debug("parentFolderId: " + parentFolderId);
                }
            }

            return exchFolder;
        }

        public ExchFolder GetFolder(string uniqueId)
        {
            try
            {
                return new ExchFolder(Folder.Bind(_exchangeService, new FolderId(uniqueId)));
            }
            catch
            {
                return null;
            }
        }

        public List<ExchFolder> FindFolders(string parentFolderId, string folderName, int maxResults = 1000)
        {
            var folders = _exchangeService.FindFolders(
                new FolderId(parentFolderId),
                new SearchFilter.ContainsSubstring(
                    FolderSchema.DisplayName,
                    folderName,
                    ContainmentMode.FullString,
                    ComparisonMode.IgnoreCase),
                new FolderView(maxResults) {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly, FolderSchema.DisplayName),
                    Traversal = FolderTraversal.Shallow
                }).Folders;


            var exchangeFolderList = new List<ExchFolder>();
            foreach (var folder in folders)
            {
                var exchFolder = new ExchFolder(folder);
                _logger.Debug(exchFolder);
                exchangeFolderList.Add(exchFolder);
            }

            return exchangeFolderList;
        }

        public ExchFolder GetWellKnownFolder(string folderName, string mailboxEmailAddress)
        {
            var wellKnownFolderName = WellKnownFolderName.MsgFolderRoot;
            ExchFolder exchangeFolderResult = null;

            if (!Enum.TryParse(folderName, out wellKnownFolderName))
                return exchangeFolderResult;

            var folder = Folder.Bind(_exchangeService, new FolderId(wellKnownFolderName, mailboxEmailAddress));
            if (folder != null)
                exchangeFolderResult = new ExchFolder(folder);

            return exchangeFolderResult;
        }

        public void DeleteFolder(ExchFolder exchangeFolder, bool moveToDeletedItems = true)
        {
            var folderToDelete = Folder.Bind(_exchangeService, new FolderId(exchangeFolder.UniqueId));

            if (moveToDeletedItems && !string.IsNullOrEmpty(exchangeFolder.Name))
            {
                var folderView = new FolderView(1);
                folderView.PropertySet = new PropertySet(BasePropertySet.IdOnly, FolderSchema.DisplayName);
                folderView.Traversal = FolderTraversal.Shallow;
                var searchFilter = new SearchFilter.ContainsSubstring(
                    FolderSchema.DisplayName,
                    exchangeFolder.Name,
                    ContainmentMode.FullString,
                    ComparisonMode.IgnoreCase);
                var foldersInDeletedItems = _exchangeService.FindFolders(WellKnownFolderName.DeletedItems, searchFilter, folderView).Folders;
                if (foldersInDeletedItems.Count > 0)
                    foldersInDeletedItems[0].Delete(DeleteMode.HardDelete);

                folderToDelete.Delete(DeleteMode.MoveToDeletedItems);
            }

            folderToDelete.Delete(DeleteMode.HardDelete);
        }

        public ExchFolder CreateFolder(ExchFolder parentFolder, string foldername)
        {
            var folderToCreate = new Folder(_exchangeService);
            folderToCreate.DisplayName = foldername;
            folderToCreate.Save(new FolderId(parentFolder.UniqueId));

            return new ExchFolder(folderToCreate);
        }
    }
}
