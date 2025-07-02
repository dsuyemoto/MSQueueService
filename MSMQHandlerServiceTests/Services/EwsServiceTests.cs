using EWS;
using LoggerHelper;
using Moq;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using NUnit.Framework;
using QueueService;
using QueueServiceWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Tests.Services
{
    [TestFixture()]
    public class EwsServiceTests
    {
        IEwsWrapper _mockEwsWrapper;
        CancellationTokenSource _cancellationTokenSource;
        IQueueService _mockQueueService;
        EwsCredsQueue _ewsCreds;
        EwsFolderQueue _ewsFolder;
        string _messageIdBase64;
        ICacheService _mockCacheService;
        ILogger _mockLogger;
        ExchEmail _email1;
        ExchEmail _email2;
        ExchEmail _email3;
        ExchFolder _exchFolder;
        List<ExchEmail> _exchEmails;
        byte[] _emailBytes;
        byte[] _attachmentBytes;

        const string MAILBOXEMAILADDRESS = "MailboxEmailAddress";
        const string PASSWORDBASE64 = "UGFzc3dvcmQ=";
        const string USERNAME = "Username";
        const string MESSAGEID = "messageid";
        const string UNIQUEID = "UniqueId";
        const string UNIQUEID2 = "UniqueId2";
        const string UNIQUEID3 = "UniqueId3";
        const string PARENTUNIQUEFOLDERID = "ParentUniqueFolderID";
        const string UNIQUEFOLDERID = "UniqueFolderId";
        const string FOLDERNAME = "FolderName";
        const string FOLDERPATH = "Folderpath";
        const string ATTACHMENTNAME = "AttachmentName";

        public EwsServiceTests()
        {
            _emailBytes = new byte[] { 1, 2, 3, 4, 5 };
            _attachmentBytes = new byte[] { 1, 2, 3, 4 };
            _messageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _cancellationTokenSource = new CancellationTokenSource();
            _ewsFolder = new EwsFolderQueue() 
            { 
                Name = FOLDERNAME,
                ParentFolderUniqueId = PARENTUNIQUEFOLDERID, 
                UniqueId = UNIQUEFOLDERID 
            };
            _ewsCreds = new EwsCredsQueue()
            {
                AutodiscoverEmailAddress = MAILBOXEMAILADDRESS,
                Username = USERNAME,
                PasswordBase64 = PASSWORDBASE64
            };
            _email1 = new ExchEmail()
            {
                UniqueId = UNIQUEID,
                ParentFolderUniqueId = PARENTUNIQUEFOLDERID,
                Content = _emailBytes,
                Attachments = new List<IExchAttachment>()
                {
                    new ExchAttachmentEmail()
                    {
                        FileName = ATTACHMENTNAME,
                        Content = _attachmentBytes
                    }
                }
            };
            _email2 = new ExchEmail()
            {
                UniqueId = UNIQUEID2,
                ParentFolderUniqueId = PARENTUNIQUEFOLDERID
            };
            _email3 = new ExchEmail()
            {
                UniqueId = UNIQUEID3,
                ParentFolderUniqueId = PARENTUNIQUEFOLDERID
            };
            _exchFolder = new ExchFolder()
            {
                UniqueId = UNIQUEFOLDERID,
                ParentFolderUniqueId = PARENTUNIQUEFOLDERID
            };
            _exchEmails = new List<ExchEmail>();
            _exchEmails.Add(_email1);
            _exchEmails.Add(_email2);
            _exchEmails.Add(_email3);
        }

        [SetUp()]
        public void Setup()
        {           
            _mockEwsWrapper = Mock.Of<IEwsWrapper>();
            _mockLogger = Mock.Of<ILogger>();
            _mockQueueService = Mock.Of<IQueueService>();
            _mockCacheService = Mock.Of<ICacheService>();
            var ewsFolderResult = new EwsResultFolderQueue() { Folder = _ewsFolder };

            Mock.Get(_mockEwsWrapper)
                .Setup(e => e.GetFolder(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new ExchFolder());
            Mock.Get(_mockEwsWrapper).Setup(e => e.GetFolder(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<int>())).Returns(_exchFolder);
            Mock.Get(_mockEwsWrapper).Setup(e => e.GetEmails(It.IsAny<ExchFolder>(), It.IsAny<int>()))
                .Returns(_exchEmails);
            Mock.Get(_mockEwsWrapper).Setup(e => e.DeleteFolder(It.IsAny<ExchFolder>(), It.IsAny<bool>()));
            Mock.Get(_mockEwsWrapper).Setup(e => e.GetEmail(It.IsAny<string>())).Returns(_email1);
            Mock.Get(_mockQueueService).Setup(q => q.RequestAsync(It.IsAny<MsmqMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => MESSAGEID));
            Mock.Get(_mockQueueService)
                .Setup(q => q.ResponseAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => { return new object(); }));
            Mock.Get(_mockQueueService)
                .Setup(e => e.GetMessageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => { return (object)ewsFolderResult; }));
            Mock.Get(_mockCacheService)
                .Setup(c => c.GetConnectionAsync(
                    It.IsAny<object>(), 
                    It.IsAny<Func<object>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => (object)_mockEwsWrapper));
        }

        [Test()]
        public void GetEmail_UniqueId_AreEqualTest()
        {
            var result = (EwsResultEmailQueue)new EwsService(_mockQueueService, _mockCacheService, _mockLogger).HandlerCallbackAsync(
                new EwsGetEmailQueue() { Creds = _ewsCreds, UniqueId = UNIQUEID },
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(UNIQUEID, result.Emails[0].UniqueId);
        }

        [Test()]
        public void GetEmailsFromQueue_UniqueIds_AreEqualTest()
        {
            var result = (EwsResultEmailQueue)new EwsService(_mockQueueService, _mockCacheService, _mockLogger).HandlerCallbackAsync(
                new EwsGetEmailsQueue()
                {
                    FolderMessageIdBase64 = _messageIdBase64,
                    Creds = _ewsCreds,
                    MaxWaitTimeSeconds = 0
                },
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(_email1.UniqueId, result.Emails[0].UniqueId);
            Assert.AreEqual(_email2.UniqueId, result.Emails[1].UniqueId);
            Assert.AreEqual(_email3.UniqueId, result.Emails[2].UniqueId);
        }

        [Test()]
        public void GetFolder_UniqueId_AreEqualTest()
        {
            var result = (EwsResultFolderQueue)new EwsService(_mockQueueService, _mockCacheService, _mockLogger).HandlerCallbackAsync(
                new EwsGetFolderQueue()
                {
                    FolderPath = FOLDERPATH,
                    Creds = _ewsCreds,
                    MailboxEmailAddress = MAILBOXEMAILADDRESS
                },
                _cancellationTokenSource.Token).Result;

            Assert.AreEqual(UNIQUEFOLDERID, result.Folder.UniqueId);
        }

        [Test()]
        public void DeleteFolder_FolderId_AreEqualTest()
        {
            var result = (EwsResultFolderQueue)new EwsService(_mockQueueService, _mockCacheService, _mockLogger).HandlerCallbackAsync(
                new EwsDeleteFolderQueue() { Creds = _ewsCreds, MessageIdBase64 = _messageIdBase64 },
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(UNIQUEFOLDERID, result.Folder.UniqueId);
        }

        [Test()]
        public void DownloadEmailContent_ContentBytes_AreEqualTest()
        {
            var result = new EwsService(_mockQueueService, _mockCacheService, _mockLogger).DownloadEmailAsync(
                UNIQUEID,
                _ewsCreds,
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(_emailBytes, result);
        }

        [Test()]
        public void DownloadEmailAttachment_ContentBytes_AreEqualTest()
        {
            var result = new EwsService(_mockQueueService, _mockCacheService, _mockLogger).DownloadAttachmentsAsync(
                new EwsEmailQueue(),
                _ewsCreds,
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(_attachmentBytes, result[0].Content);
        }
    }
}
