using NUnit.Framework;
using System;
using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace EWS.Tests
{
    [TestFixture()]
    public class EwsEmailWrapperTests
    {
        EwsWrapper _ewsWrapper;
        const string INTEGRATIONTESTINGFOLDERNAME = "Integration Testing";
        const string ARCHIVEFOLDERSNAME = "ArchiveFolders";
        ExchFolder _archiveFolder;
        ExchFolder _integrationTestingFolder;

        public EwsEmailWrapperTests()
        {
            var ewsWrapperConnector = new EwsWrapperConnector();
            _ewsWrapper = ewsWrapperConnector.Instance();
            _archiveFolder = _ewsWrapper.FindFolder(_ewsWrapper.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot), ARCHIVEFOLDERSNAME);
            _integrationTestingFolder = _ewsWrapper.FindFolder(_archiveFolder, INTEGRATIONTESTINGFOLDERNAME);
        }

        [Test()]
        public void GetEmails_NoFilter_IntegrationsTest()
        {
            var emailsFound = (List<ExchEmail>)_ewsWrapper.GetEmails(_integrationTestingFolder);

            Assert.Greater(emailsFound.Count, 0);
        }

        [Test()]
        public void GetEmails_WithFilter_IntegrationsTest1()
        {
            var subject = "email";
            var exchSearchFilter = new ExchSearchFilter();
            exchSearchFilter.ContainsSubstring(ItemSchema.Subject, subject, ContainmentMode.Substring, ComparisonMode.IgnoreCase);

            var emailsFound = (List<ExchEmail>)_ewsWrapper.GetEmails(_integrationTestingFolder, exchSearchFilter);

            Assert.AreEqual(1, emailsFound.Count);
        }
    }
}