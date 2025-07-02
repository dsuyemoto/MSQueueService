using EWS;
using Imanage;
using LoggerHelper;
using Moq;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using QueueService;
using QueueServiceWebApp.Controllers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Imanage.DocumentProfileItems;

namespace MSMQHandlerService.Tests.Services
{
    [TestFixture()]
    public class ImanageServiceTests
    {
        IImanageRepository _mockImanageRepository;
        CancellationTokenSource _cancellationTokenSource;
        IQueueService _mockQueueServiceImanage;
        IQueueService _mockQueueServiceEws;
        ImanageDocumentQueue _sourceDocument;
        ImanageEmailPropertiesQueue _imanageEmailPropertiesQueue;
        ImanageDocumentOutput _imanageDocumentOutput;
        ICacheService _mockCacheService;
        List<ImanageDocumentQueue> _resultDocuments;
        ImanageDocumentsOutput _imanageDocumentsOutput;
        ImanageSetDocumentsPropertiesInput _imanageSetDocumentsInput;
        IFileService _mockFileService;
        IEwsService _mockEwsService;
        ILogger _mockLogger;

        string _commentBase64;
        string _descriptionBase64;
        string _contentBase64;
        string _passwordBase64;
        string _messageIdBase64;
        string _imanageMessageId;
        string _ewsMessageId;
        byte[] _content;
        DateTime _sent;
        DateTime _received;

        const string SESSION = "session";
        const string DATABASE = "US-DOCS";
        const string NUMBER = "number";
        const string VERSION = "1";
        const string FOLDERID = "folderid";
        const string AUTHOR = "author";
        const string OPERATOR = "operator";
        const string EMAIL = "EMAIL";
        const string EML = "EML";
        const string DESCRIPTION = "description";
        const string COMMENT = "comment";
        const string CCNAMES = "CcNames";
        const string FROMNAME = "FromName";
        const string TONAMES = "ToNames";
        const string USERNAME = "Username";
        const string PASSWORD = "Password";
        const string CLASS = "DOC";
        const string TYPE = "ACROBAT";
        const string MESSAGEID = "messageid";
        const string UNIQUEID = "uniqueid";
        const string ERRORMESSAGE = "ErrorMessage";
        const string IMANAGEPROFILEERROR = "ImanageProfileError";
        const string EXTENSION = "EXT";
        const string WARNINGFIELD = "Field";
        const string FILEPATH = @"c:\Temp\test.pdf";
        const string APPDOMAIN = "MSMQHandlerServiceTests";

        public ImanageServiceTests()
        {
            _sent = DateTime.Now;
            _received = DateTime.Now;
            _content = new byte[1];
            _descriptionBase64 = ControllerHelpers.Base64ConvertTo(DESCRIPTION);
            _commentBase64 = ControllerHelpers.Base64ConvertTo(COMMENT);
            _contentBase64 = Convert.ToBase64String(_content);
            _passwordBase64 = ControllerHelpers.Base64ConvertTo(PASSWORD);
            _messageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _cancellationTokenSource = new CancellationTokenSource();
            _imanageEmailPropertiesQueue = new ImanageEmailPropertiesQueue()
            {
                CcNames = CCNAMES,
                FromName = FROMNAME,
                ReceivedDate = _received.ToString(),
                SentDate = _sent.ToString(),
                ToNames = TONAMES
            };
            _sourceDocument = new ImanageDocumentQueue()
            {
                Author = AUTHOR,
                CommentBase64 = _commentBase64,
                ContentBytesBase64 = _contentBase64,
                Database = DATABASE,
                DescriptionBase64 = _descriptionBase64,
                EmailProperties = _imanageEmailPropertiesQueue,
                Operator = OPERATOR,
                SecurityPasswordBase64 = _passwordBase64,
                SecurityUsername = USERNAME,
                Session = SESSION,
                Extension = EXTENSION
            };
        }

        [SetUp()]
        public void Setup()
        {
            _mockLogger = Mock.Of<ILogger>();
            _ewsMessageId = string.Empty;
            _imanageMessageId = string.Empty;
            _imanageDocumentOutput = new ImanageDocumentOutput(
                DATABASE,
                SESSION,
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(
                        new DocumentResponseSingle()
                        {
                            data = new DocumentResponseSingleData()
                            {
                                Author = AUTHOR,
                                Class = CLASS,
                                Comment = COMMENT,
                                Name = DESCRIPTION,
                                Number = NUMBER,
                                Operator = OPERATOR,
                                Type = TYPE,
                                Version = VERSION,
                                Extension = EXTENSION,
                                From = FROMNAME,
                                To = TONAMES,
                                Cc = CCNAMES,
                                Sent = _sent.ToString(),
                                Received = _received.ToString()
                            },
                            warnings = new DocumentResponseSingleWarnings[] {
                                new DocumentResponseSingleWarnings(WARNINGFIELD, IMANAGEPROFILEERROR)
                            }

                        }),
                    ErrorMessage = ERRORMESSAGE,
                    StatusCode = HttpStatusCode.OK
                });
            _imanageDocumentsOutput = new ImanageDocumentsOutput()
            {
                Documents = new ImanageDocumentOutput[] { _imanageDocumentOutput },
                Errors = new string[] { ERRORMESSAGE }
            };
            _resultDocuments = new List<ImanageDocumentQueue>();
            _resultDocuments.Add(new ImanageDocumentQueue()
            {
                Author = _imanageDocumentOutput.DocumentProfileItems.Author,
                ContentBytesBase64 = null,
                CommentBase64 = ControllerHelpers.Base64ConvertTo(_imanageDocumentOutput.DocumentProfileItems.Comment),
                Database = _imanageDocumentOutput.Database,
                DescriptionBase64 = ControllerHelpers.Base64ConvertTo(_imanageDocumentOutput.DocumentProfileItems.Description),
                ImanageError = null,
                Number = _imanageDocumentOutput.DocumentObjectId.Number,
                Version = _imanageDocumentOutput.DocumentObjectId.Version,
                Operator = _imanageDocumentOutput.DocumentProfileItems.Operator,
                Session = _imanageDocumentOutput.DocumentObjectId.Session
            });
            _mockImanageRepository = Mock.Of<IImanageRepository>();
            Mock.Get(_mockImanageRepository)
                .Setup(i => i.CreateDocuments(It.IsAny<ImanageCreateDocumentsInput>()))
                .Returns(_imanageDocumentsOutput);
            Mock.Get(_mockImanageRepository)
                .Setup(i => i.UpdateDocuments(It.IsAny<ImanageSetDocumentsPropertiesInput>()))
                .Callback<ImanageSetDocumentsPropertiesInput>((c) => _imanageSetDocumentsInput = c)
                .Returns(_imanageDocumentsOutput);
            var emails = new List<ExchEmail>();
            emails.Add(new ExchEmail() { UniqueId = UNIQUEID });
            _mockEwsService = Mock.Of<IEwsService>();
            Mock.Get(_mockEwsService)
                .Setup(e => e.HandlerCallbackAsync(It.IsAny< EwsGetEmailsQueue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(()=> {
                    return (object)new EwsResultEmailQueue() {
                        Emails = new EwsEmailQueue[] { 
                            new EwsEmailQueue() { EwsFolder = new EwsFolderQueue() }
                        } 
                    }; 
                }));
            Mock.Get(_mockEwsService)
                .Setup(e => e.HandlerCallbackAsync(It.IsAny<EwsDeleteFolderQueue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(()=> { return (object)new EwsResultFolderQueue(); }));
            //Mock.Get(_mockEwsService)
            //    .Setup(ew => ew.GetEmail(It.IsAny<string>()))
            //    .Returns(new ExchEmail() { UniqueId = UNIQUEID, Content = _content });
            _mockQueueServiceEws = Mock.Of<IQueueService>();
            Mock.Get(_mockQueueServiceEws).Setup(e => e.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Callback<string, CancellationToken, bool>((m, t, b) => _ewsMessageId = m)
                .Returns(Task.Run(() => { return (object)new EwsResultEmailQueue(); }));
            _mockQueueServiceImanage = Mock.Of<IQueueService>();
            Mock.Get(_mockQueueServiceImanage)
                .Setup(q => q.GetMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .Callback<string, CancellationToken, bool>((m, t, b) => _imanageMessageId = m)
                .Returns(Task.Run(() =>
                {
                    return (object)new ImanageResultDocumentQueue()
                    {
                        Documents = _resultDocuments.ToArray()
                    };
                }));
            _mockCacheService = Mock.Of<ICacheService>();
            Mock.Get(_mockCacheService)
                .Setup(c => c.GetConnectionAsync(
                    It.IsAny<object>(), 
                    It.IsAny<Func<object>>(), 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() =>(object)_mockImanageRepository));
            _mockFileService = Mock.Of<IFileService>();
            Mock.Get(_mockFileService).Setup(f => f.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(()=> { return _content; }));
            Mock.Get(_mockFileService).Setup(f => f.Exists(It.IsAny<string>()))
                .Returns(true);
        }

        [Test()]
        public void CreateDocument_Results_AreEqualTest()
        {
            var result = new ImanageService(
                _mockQueueServiceImanage, 
                _mockQueueServiceEws,
                _mockCacheService, 
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageCreateDocumentQueue()
                    {
                        Document = _sourceDocument,
                        SourceEmail = new ImanageSourceEmailQueue()
                        {
                            FolderUniqueId = UNIQUEID,
                            DeleteSourceFolder = true,
                            Creds = new EwsCredsQueue()
                        },
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Result;
            var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;

            Assert.AreEqual(FOLDERID, imanageResultDocumentQueue.FolderId);
            Assert.AreEqual(AUTHOR, imanageResultDocumentQueue.Documents[0].Author);
            Assert.AreEqual(OPERATOR, imanageResultDocumentQueue.Documents[0].Operator);
            Assert.AreEqual(_commentBase64, imanageResultDocumentQueue.Documents[0].CommentBase64);
            Assert.AreEqual(_descriptionBase64, imanageResultDocumentQueue.Documents[0].DescriptionBase64);
            Assert.AreEqual(DATABASE, imanageResultDocumentQueue.Documents[0].Database);
            Assert.AreEqual(CCNAMES, imanageResultDocumentQueue.Documents[0].EmailProperties.CcNames);
            Assert.AreEqual(TONAMES, imanageResultDocumentQueue.Documents[0].EmailProperties.ToNames);
            Assert.AreEqual(FROMNAME, imanageResultDocumentQueue.Documents[0].EmailProperties.FromName);
            Assert.AreEqual(_received.ToString(), imanageResultDocumentQueue.Documents[0].EmailProperties.ReceivedDate);
            Assert.AreEqual(_sent.ToString(), imanageResultDocumentQueue.Documents[0].EmailProperties.SentDate);
            Assert.AreEqual(DESCRIPTION, imanageResultDocumentQueue.Documents[0].EmailProperties.Subject);
            Assert.AreEqual(IMANAGEPROFILEERROR, imanageResultDocumentQueue.Documents[0].ImanageError.ProfileErrors[0].Message);
            Assert.AreEqual(SESSION, imanageResultDocumentQueue.Documents[0].Session);
            Assert.AreEqual(NUMBER, imanageResultDocumentQueue.Documents[0].Number);
            Assert.AreEqual(VERSION, imanageResultDocumentQueue.Documents[0].Version);
            Assert.AreEqual(EXTENSION, imanageResultDocumentQueue.Documents[0].Extension);
        }

        public void CreateDocument_SourceEmailMessageId_Results_AreEqualTest()
        {
            var result = new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageCreateDocumentQueue()
                    {
                        Document = _sourceDocument,
                        SourceEmail = new ImanageSourceEmailQueue()
                        {
                            MessageIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MESSAGEID)),
                            DeleteSourceFolder = true,
                            Creds = new EwsCredsQueue()
                        },
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Result;
            var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;

            Assert.AreEqual(FOLDERID, imanageResultDocumentQueue.FolderId);
            Assert.AreEqual(AUTHOR, imanageResultDocumentQueue.Documents[0].Author);
            Assert.AreEqual(OPERATOR, imanageResultDocumentQueue.Documents[0].Operator);
            Assert.AreEqual(_commentBase64, imanageResultDocumentQueue.Documents[0].CommentBase64);
            Assert.AreEqual(_descriptionBase64, imanageResultDocumentQueue.Documents[0].DescriptionBase64);
            Assert.AreEqual(DATABASE, imanageResultDocumentQueue.Documents[0].Database);
            Assert.AreEqual(CCNAMES, imanageResultDocumentQueue.Documents[0].EmailProperties.CcNames);
            Assert.AreEqual(TONAMES, imanageResultDocumentQueue.Documents[0].EmailProperties.ToNames);
            Assert.AreEqual(FROMNAME, imanageResultDocumentQueue.Documents[0].EmailProperties.FromName);
            Assert.AreEqual(_received.ToString(), imanageResultDocumentQueue.Documents[0].EmailProperties.ReceivedDate);
            Assert.AreEqual(_sent.ToString(), imanageResultDocumentQueue.Documents[0].EmailProperties.SentDate);
            Assert.AreEqual(DESCRIPTION, imanageResultDocumentQueue.Documents[0].EmailProperties.Subject);
            Assert.AreEqual(IMANAGEPROFILEERROR, imanageResultDocumentQueue.Documents[0].ImanageError.ProfileErrors[0].Message);
            Assert.AreEqual(SESSION, imanageResultDocumentQueue.Documents[0].Session);
            Assert.AreEqual(NUMBER, imanageResultDocumentQueue.Documents[0].Number);
            Assert.AreEqual(VERSION, imanageResultDocumentQueue.Documents[0].Version);
            Assert.AreEqual(EXTENSION, imanageResultDocumentQueue.Documents[0].Extension);
        }

        public void CreateDocument_SourceFilePath_Results_AreEqualTest()
        {
            var result = new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageCreateDocumentQueue()
                    {
                        Document = _sourceDocument,
                        SourceFilePath = FILEPATH,
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Result;
            var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;

            Assert.AreEqual(FOLDERID, imanageResultDocumentQueue.FolderId);
            Assert.AreEqual(AUTHOR, imanageResultDocumentQueue.Documents[0].Author);
            Assert.AreEqual(OPERATOR, imanageResultDocumentQueue.Documents[0].Operator);
            Assert.AreEqual(_commentBase64, imanageResultDocumentQueue.Documents[0].CommentBase64);
            Assert.AreEqual(_descriptionBase64, imanageResultDocumentQueue.Documents[0].DescriptionBase64);
            Assert.AreEqual(DATABASE, imanageResultDocumentQueue.Documents[0].Database);
            Assert.AreEqual(CCNAMES, imanageResultDocumentQueue.Documents[0].EmailProperties.CcNames);
            Assert.AreEqual(TONAMES, imanageResultDocumentQueue.Documents[0].EmailProperties.ToNames);
            Assert.AreEqual(FROMNAME, imanageResultDocumentQueue.Documents[0].EmailProperties.FromName);
            Assert.AreEqual(_received.ToString(), imanageResultDocumentQueue.Documents[0].EmailProperties.ReceivedDate);
            Assert.AreEqual(_sent.ToString(), imanageResultDocumentQueue.Documents[0].EmailProperties.SentDate);
            Assert.AreEqual(DESCRIPTION, imanageResultDocumentQueue.Documents[0].EmailProperties.Subject);
            Assert.AreEqual(IMANAGEPROFILEERROR, imanageResultDocumentQueue.Documents[0].ImanageError.ProfileErrors[0].Message);
            Assert.AreEqual(SESSION, imanageResultDocumentQueue.Documents[0].Session);
            Assert.AreEqual(NUMBER, imanageResultDocumentQueue.Documents[0].Number);
            Assert.AreEqual(VERSION, imanageResultDocumentQueue.Documents[0].Version);
            Assert.AreEqual(EXTENSION, imanageResultDocumentQueue.Documents[0].Extension);
        }

        [Test]
        public void CreateDocument_SourceFilePath_FileRead_CalledOnceTest()
        {
            var result = new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageCreateDocumentQueue()
                    {
                        Document = new ImanageDocumentQueue()
                        {
                            Author = AUTHOR,
                            CommentBase64 = _commentBase64,
                            Database = DATABASE,
                            DescriptionBase64 = _descriptionBase64,
                            Operator = OPERATOR,
                            SecurityPasswordBase64 = _passwordBase64,
                            SecurityUsername = USERNAME,
                            Session = SESSION
                        },
                        SourceFilePath = FILEPATH,
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Result;
            var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;

            Mock.Get(_mockFileService).Verify(f => f.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void CreateDocument_ErrorMessage_AreEqualTest()
        {
            _imanageDocumentOutput = new ImanageDocumentOutput(
                DATABASE,
                SESSION,
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(
                        new DocumentResponseSingle()
                        {
                            data = new DocumentResponseSingleData()
                            {
                                Author = AUTHOR,
                                Class = CLASS,
                                Comment = COMMENT,
                                Name = DESCRIPTION,
                                Number = NUMBER,
                                Operator = OPERATOR,
                                Type = TYPE,
                                Version = VERSION,
                                Extension = EXTENSION,
                                From = FROMNAME,
                                To = TONAMES,
                                Cc = CCNAMES,
                                Sent = _sent.ToString(),
                                Received = _received.ToString()
                            },
                            warnings = new DocumentResponseSingleWarnings[] {
                                new DocumentResponseSingleWarnings(WARNINGFIELD, IMANAGEPROFILEERROR)
                            }

                        }),
                    ErrorMessage = ERRORMESSAGE,
                    StatusCode = HttpStatusCode.NoContent
                });
            _imanageDocumentsOutput = new ImanageDocumentsOutput()
            {
                Documents = new ImanageDocumentOutput[] { _imanageDocumentOutput },
                Errors = new string[] { ERRORMESSAGE }
            };
            Mock.Get(_mockImanageRepository)
                .Setup(i => i.CreateDocuments(It.IsAny<ImanageCreateDocumentsInput>()))
                .Returns(_imanageDocumentsOutput);

            var result = new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageCreateDocumentQueue()
                    {
                        Document = _sourceDocument,
                        SourceEmail = new ImanageSourceEmailQueue()
                        {
                            FolderUniqueId = UNIQUEID,
                            DeleteSourceFolder = true,
                            Creds = new EwsCredsQueue()
                        },
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Result;
            var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;

            Assert.AreEqual(ERRORMESSAGE, imanageResultDocumentQueue.Documents[0].ImanageError.Message);
        }

        [Test()]
        public void GetDocument_GetDocument_CalledOnceTest()
        {
            Mock.Get(_mockImanageRepository)
                .Setup(i => i.GetDocuments(It.IsAny<ImanageGetDocumentsInput>()))
                .Returns(new ImanageDocumentsOutput() { Documents = new ImanageDocumentOutput[] { _imanageDocumentOutput } });

            new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService,
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageGetDocumentQueue()
                    {
                        Database = DATABASE,
                        Session = SESSION,
                        Number = NUMBER,
                        Version = VERSION,
                        SecurityUsername = USERNAME,
                        SecurityPasswordBase64 = _passwordBase64
                    },
                _cancellationTokenSource.Token).Wait();

            Mock.Get(_mockImanageRepository).Verify(i => i.GetDocuments(It.IsAny<ImanageGetDocumentsInput>()), Times.Once);
        }

        [Test()]
        public void UpdateDocument_DocumentToDocumentProfileItems_AreEqualTest()
        {
            _sourceDocument = new ImanageDocumentQueue()
            {
                Author = AUTHOR,
                Operator = OPERATOR,
                DescriptionBase64 = _descriptionBase64,
                CommentBase64 = _commentBase64,
                ContentBytesBase64 = _contentBase64,
                EmailProperties = _imanageEmailPropertiesQueue,
                SecurityUsername = USERNAME,
                SecurityPasswordBase64 = _passwordBase64,
                Session = SESSION,
                Database = DATABASE
            };

            var result = new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageUpdateDocumentQueue()
                    {
                        Document = _sourceDocument,
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Result;
            var imanageDocumentQueue = ((ImanageResultDocumentQueue)result).Documents[0];

            Assert.AreEqual(AUTHOR, imanageDocumentQueue.Author);
            Assert.AreEqual(OPERATOR, imanageDocumentQueue.Operator);
            Assert.AreEqual(_descriptionBase64, imanageDocumentQueue.DescriptionBase64);
            Assert.AreEqual(_commentBase64, imanageDocumentQueue.CommentBase64);
            Assert.AreEqual(SESSION, imanageDocumentQueue.Session);
            Assert.AreEqual(DATABASE, imanageDocumentQueue.Database);
        }

        [Test()]
        public void UpdateDocument_ImanageMessageIdBase64_IsValidTest()
        {
            _sourceDocument = new ImanageDocumentQueue()
            {
                Author = AUTHOR,
                Operator = OPERATOR,
                DescriptionBase64 = _descriptionBase64,
                CommentBase64 = _commentBase64,
                ContentBytesBase64 = _contentBase64,
                EmailProperties = _imanageEmailPropertiesQueue,
                SecurityUsername = USERNAME,
                SecurityPasswordBase64 = _passwordBase64,
                Session = SESSION,
                Database = DATABASE
            };

            new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageUpdateDocumentQueue()
                    {
                        MessageIdBase64 = _messageIdBase64,
                        Document = _sourceDocument,
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Wait();

            Assert.IsNotEmpty(_imanageMessageId);
        }

        [Test()]
        public void UpdateDocument_EwsMessageIdBase64_IsValidTest()
        {
            _sourceDocument = new ImanageDocumentQueue()
            {
                Author = AUTHOR,
                Operator = OPERATOR,
                DescriptionBase64 = _descriptionBase64,
                CommentBase64 = _commentBase64,
                EmailProperties = _imanageEmailPropertiesQueue,
                SecurityUsername = USERNAME,
                SecurityPasswordBase64 = _passwordBase64,
                Session = SESSION,
                Database = DATABASE
            };

            new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageUpdateDocumentQueue()
                    {
                        SourceEmail = new ImanageSourceEmailQueue() { MessageIdBase64 = _messageIdBase64 },
                        Document = _sourceDocument,
                        FolderId = FOLDERID
                    },
                _cancellationTokenSource.Token).Wait();

            Assert.IsNotEmpty(_ewsMessageId);
        }

        [Test()]
        public void UpdateDocument_GetMessageAsync_CalledOnceTest()
        {
            _sourceDocument = new ImanageDocumentQueue()
            {
                Author = AUTHOR,
                Operator = OPERATOR,
                DescriptionBase64 = _descriptionBase64,
                CommentBase64 = _commentBase64,
                EmailProperties = _imanageEmailPropertiesQueue,
                SecurityUsername = USERNAME,
                SecurityPasswordBase64 = _passwordBase64,
                Session = SESSION,
                Database = DATABASE
            };

            new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageUpdateDocumentQueue()
                    {
                        MessageIdBase64 = _messageIdBase64,
                        Document = _sourceDocument
                    },
                _cancellationTokenSource.Token).Wait();

            Mock.Get(_mockQueueServiceImanage)
                .Verify(q => q.GetMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(), 
                    It.IsAny<bool>()), 
                    Times.Once);
        }

        [Test()]
        public void GetDocument_DocumentProfileItemsToDocument_AreEqualTest()
        {
            Mock.Get(_mockImanageRepository)
                .Setup(i => i.GetDocuments(It.IsAny<ImanageGetDocumentsInput>()))
                .Returns(new ImanageDocumentsOutput()
                {
                    Documents = new ImanageDocumentOutput[] { _imanageDocumentOutput }
                });

            var result = new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                    new ImanageGetDocumentQueue()
                    {
                        Database = DATABASE,
                        Session = SESSION,
                        Number = NUMBER,
                        Version = VERSION,
                        SecurityUsername = USERNAME,
                        SecurityPasswordBase64 = _passwordBase64
                    },
                _cancellationTokenSource.Token).Result;
            var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;
            var imanageDocumentResultQueue = imanageResultDocumentQueue.Documents[0];

            Assert.AreEqual(AUTHOR, imanageDocumentResultQueue.Author);
            Assert.AreEqual(OPERATOR, imanageDocumentResultQueue.Operator);
            Assert.AreEqual(_descriptionBase64, imanageDocumentResultQueue.DescriptionBase64);
            Assert.AreEqual(_commentBase64, imanageDocumentResultQueue.CommentBase64);
            Assert.AreEqual(SESSION, imanageDocumentResultQueue.Session);
            Assert.AreEqual(DATABASE, imanageDocumentResultQueue.Database);
        }

        [Test()]
        public void CreateDocument_DeclareAsRecordExtensions_AreEqualTest()
        {
            var collection = new StringCollection();
            collection.Add(ProfileApplicationType.EML.ToString());
            ImanageService.SetDeclareAsRecordExtensions(collection);

            new ImanageService(
                _mockQueueServiceImanage,
                _mockQueueServiceEws,
                _mockCacheService,
                _mockFileService, 
                _mockEwsService,
                _mockLogger)
                .HandlerCallbackAsync(
                   new ImanageCreateDocumentQueue()
                   {
                       Document = new ImanageDocumentQueue()
                       {
                           ContentBytesBase64 = _contentBase64,
                           Database = DATABASE,
                           Session = SESSION,
                           SecurityUsername = USERNAME,
                           SecurityPasswordBase64 = _passwordBase64,
                           Extension = "EML"
                       },
                       FolderId = FOLDERID,
                       SourceEmail = new ImanageSourceEmailQueue()
                   },
               _cancellationTokenSource.Token).Wait();

            Assert.IsTrue(_imanageSetDocumentsInput.ImanageDocumentsSet[0].DocumentProfileItems.DeclareAsRecord);
        }
    }
}
