using NUnit.Framework;
using Microsoft.Exchange.WebServices.Data;

namespace EWS.Tests
{
    [TestFixture()]
    public class ExchFolderTests
    {
        [Test()]
        public void ExchFolderIntegrationsTest()
        {
            const string INTEGRATIONTESTINGFOLDERNAME = "Integration Testing";
            const string ARCHIVEFOLDERSNAME = "ArchiveFolders";
            var ewsWrapperConnector = new EwsWrapperConnector();
            var ewsWrapper = ewsWrapperConnector.Instance();
            var archiveFolder = ewsWrapper.FindFolder(ewsWrapper.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot), ARCHIVEFOLDERSNAME);
            var integrationTestingFolder = ewsWrapper.FindFolder(archiveFolder, INTEGRATIONTESTINGFOLDERNAME);

            Assert.AreEqual(INTEGRATIONTESTINGFOLDERNAME, integrationTestingFolder.Name);
            Assert.IsNotEmpty(integrationTestingFolder.UniqueId);
            Assert.IsNotEmpty(integrationTestingFolder.ParentFolderUniqueId);
        }
    }
}