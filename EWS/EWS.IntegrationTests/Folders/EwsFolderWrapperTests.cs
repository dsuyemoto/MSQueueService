using NUnit.Framework;
using EWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace EWS.Tests
{
    [TestFixture()]
    public class EwsFolderWrapperTests
    {
        EwsWrapper _ewsWrapper;
        const string TESTFOLDERNAME1 = "IntegrationTest_EwsFolderWrapperTests1";
        const string TESTFOLDERNAME2 = "IntegrationTest_EwsFolderWrapperTests2";
        const string TESTFOLDERNAME3 = "IntegrationTest_EwsFolderWrapperTests3";
        const string INTEGRATIONTESTINGFOLDERNAME = "Integration Testing";
        const string ARCHIVEFOLDERSNAME = "ArchiveFolders";
        ExchFolder _archiveFolder;
        ExchFolder _integrationTestingFolder;

        public EwsFolderWrapperTests()
        {
            var ewsWrapperConnector = new EwsWrapperConnector();
            _ewsWrapper = ewsWrapperConnector.Instance();
            _archiveFolder = _ewsWrapper.FindFolder(_ewsWrapper.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot), ARCHIVEFOLDERSNAME);
            _integrationTestingFolder = _ewsWrapper.FindFolder(_archiveFolder, INTEGRATIONTESTINGFOLDERNAME);
        }

        [Test()]
        public void DeleteFolderIntegrationsTest()
        {
            var folder = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);
            if (folder == null)
                folder = _ewsWrapper.CreateFolder(_integrationTestingFolder, TESTFOLDERNAME1);

            _ewsWrapper.DeleteFolder(folder);
            var findFolder = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);

            Assert.IsNull(findFolder);
        }


        [Test()]
        public void CreateFolderIntegrationsTest()
        {
            var existingFolder = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);
            if (existingFolder != null)
                _ewsWrapper.DeleteFolder(existingFolder);

            var exchFolder = _ewsWrapper.CreateFolder(_integrationTestingFolder, TESTFOLDERNAME1);

            Assert.IsNotNull(exchFolder);
            Assert.IsNotEmpty(exchFolder.Name);
            Assert.IsNotEmpty(exchFolder.ParentFolderUniqueId);
            Assert.IsNotEmpty(exchFolder.UniqueId);
        }

        [Test()]
        public void FindFolderIntegrationsTest()
        {
            var foundFolder = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);
            if (foundFolder == null)
            {
                _ewsWrapper.CreateFolder(_integrationTestingFolder, TESTFOLDERNAME1);
                foundFolder = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);
            }

            Assert.IsNotNull(foundFolder);

            _ewsWrapper.DeleteFolder(foundFolder);
        }

        [Test()]
        public void FindFoldersIntegrationsTest()
        {
            var foundFolder1 = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);
            if (foundFolder1 == null)
            {
                _ewsWrapper.CreateFolder(_integrationTestingFolder, TESTFOLDERNAME1);
                foundFolder1 = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME1);
            }
            var foundFolder2 = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME2);
            if (foundFolder2 == null)
            {
                _ewsWrapper.CreateFolder(_integrationTestingFolder, TESTFOLDERNAME2);
                foundFolder2 = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME2);
            }
            var foundFolder3 = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME3);
            if (foundFolder3 == null)
            {
                _ewsWrapper.CreateFolder(_integrationTestingFolder, TESTFOLDERNAME3);
                foundFolder3 = _ewsWrapper.FindFolder(_integrationTestingFolder, TESTFOLDERNAME3);
            }

            Assert.IsNotNull(foundFolder1);
            Assert.IsNotNull(foundFolder2);
            Assert.IsNotNull(foundFolder3);

            _ewsWrapper.DeleteFolder(foundFolder1);
            _ewsWrapper.DeleteFolder(foundFolder2);
            _ewsWrapper.DeleteFolder(foundFolder3);
        }

        [Test()]
        public void GetWellKnownFolder_WellKnownFolder_IntegrationsTest()
        {
            var folder = _ewsWrapper.GetWellKnownFolder(WellKnownFolderName.Inbox);

            Assert.IsNotEmpty(WellKnownFolderName.Inbox.ToString(), folder.Name);
            Assert.IsNotEmpty(folder.UniqueId);
        }

        [Test()]
        public void GetWellKnownFolder_FolderName_IntegrationsTest()
        {
            var inboxName = "Inbox";
            var folder = _ewsWrapper.GetWellKnownFolder(inboxName);

            Assert.AreEqual(inboxName, folder.Name);
            Assert.IsNotEmpty(folder.UniqueId);
        }
    }
}