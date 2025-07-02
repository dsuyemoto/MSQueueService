using Imanage.Documents;
using Moq;
using NUnit.Framework;
using RestSharp;
using static Imanage.DocumentProfileItems;
using static Imanage.ImanageInput;

namespace Imanage.Tests
{
    [TestFixture]
    public class ImanageRepositoryTests
    {
        const string DATABASE = "US-DOCS";
        const string SESSION = "US-DOCS";
        const string FOLDERID = "007";
        const string NUMBER = "100";
        const string VERSION = "1";
        const string ACLNAME = "aclname";
        const string AUTHOR = "Author";
        const string COMMENT = "Comment";
        const string DESCRIPTION = "Description";
        const string IMANOPERATOR = "Operator";
        const string IMANCLASS = "class";
        const string EXTENSION = "EML";

        IImanageRest _mockImanageRest;
        ImanageCreateDocumentsInput _imanageCreateDocumentsInput;
        ImanageGetDocumentsInput _imanageGetDocumentsInput;
        ImanageFolderObjectId _imanageFolderObjectId = new ImanageFolderObjectId(SESSION, DATABASE, FOLDERID);
        ImanageDocumentObjectId _imanageDocumentObjectId = new ImanageDocumentObjectId(SESSION, DATABASE, NUMBER, VERSION);
        ImanageGetFolderContentsInput _imanageGetFolderContentsInput;
        ImanageSetDocumentsPropertiesInput _imanageSetDocumentsPropertiesInput;
        OutputMaskName[] _outputMaskNames = new OutputMaskName[] { OutputMaskName.Profile };
        ImanageAclItem _imanageAclItem;
        ImanageAclItem[] _imanageAclItems;
        ImanageSecurityObject _imanageSecurityObject;
        ImanageRepository _imanageRepository;
        ProfileAttributeId[] _profileAttributeIds;
        ImanageDocumentsOutput _imanageDocumentsOutput;
        ImanageDocumentRest _imanageUpdateBody;

        [SetUp]
        public void Setup()
        {
            _profileAttributeIds = new ProfileAttributeId[] {
                ProfileAttributeId.Author,
                ProfileAttributeId.Comment,
                ProfileAttributeId.Description,
                ProfileAttributeId.DeclareDate,
                ProfileAttributeId.Description,
                ProfileAttributeId.Operator,
                ProfileAttributeId.Class,
                ProfileAttributeId.Type,
                ProfileAttributeId.Frozen,
                ProfileAttributeId.MarkedForArchive
            };
            _imanageAclItem = new ImanageAclItem(
                ACLNAME, 
                ImanageAclItem.ImanageAccessRight.ALL,
                ImanageAclItem.ImanageAclItemType.USER
                );
            _imanageAclItems = new ImanageAclItem[] { _imanageAclItem };
            _imanageSecurityObject = new ImanageSecurityObject(_imanageAclItems, ImanageSecurityObject.SecurityType.PUBLIC);
            _imanageCreateDocumentsInput = new ImanageCreateDocumentsInput(
                _imanageFolderObjectId,
                new ImanageDocumentCreate[] {
                    new ImanageDocumentCreate(
                        new byte[1],
                        DATABASE,
                        new DocumentProfileItemsCreate(
                            AUTHOR,
                            COMMENT,
                            DESCRIPTION,
                            IMANOPERATOR,
                            EXTENSION,
                            null),
                        _imanageSecurityObject) 
                },
                _profileAttributeIds,
                _outputMaskNames,
                true);
            _imanageGetDocumentsInput = new ImanageGetDocumentsInput(
                new ImanageDocumentObjectId[] { _imanageDocumentObjectId },
                _profileAttributeIds,
                _outputMaskNames
                );
            _imanageGetFolderContentsInput = new ImanageGetFolderContentsInput(
                    _imanageFolderObjectId,
                    _profileAttributeIds,
                    _outputMaskNames);
            _imanageSetDocumentsPropertiesInput = new ImanageSetDocumentsPropertiesInput(
                    _imanageFolderObjectId,
                    new ImanageDocumentSet[] { 
                        new ImanageDocumentSet(
                            null, 
                            new DocumentProfileItemsSet(), 
                            _imanageSecurityObject,
                            _imanageDocumentObjectId) 
                    },
                    _profileAttributeIds,
                    _outputMaskNames);
            _imanageDocumentsOutput = new ImanageDocumentsOutput() 
            {
                Documents = new ImanageDocumentOutput[] 
                { 
                    new ImanageDocumentOutput(DATABASE, SESSION, new RestResponse())
                } 
            };
            _imanageUpdateBody = new ImanageDocumentRest();
            _mockImanageRest = Mock.Of<IImanageRest>();
            Mock.Get(_mockImanageRest).Setup(r => r.UpdateDocuments(It.IsAny<ImanageSetDocumentsPropertiesInput>()))
                .Returns(_imanageDocumentsOutput);
            Mock.Get(_mockImanageRest).Setup(r => r.CreateDocuments(It.IsAny<ImanageCreateDocumentsInput>()))
                .Returns(_imanageDocumentsOutput);
            _imanageRepository = new ImanageRepository(_mockImanageRest);
        }

        [Test]
        public void CreateDocuments_CreateDocuments_CalledOnce()
        {
            _imanageRepository.CreateDocuments(_imanageCreateDocumentsInput);

            Mock.Get(_mockImanageRest).Verify(i => i.CreateDocuments(It.IsAny<ImanageCreateDocumentsInput>()), Times.Once);
        }

        [Test]
        public void GetDocuments_GetDocuments_CalledOnce()
        {
            _imanageRepository.GetDocuments(_imanageGetDocumentsInput);

            Mock.Get(_mockImanageRest).Verify(i => i.GetDocuments(It.IsAny<ImanageGetDocumentsInput>()), Times.Once);
        }

        [Test]
        public void UpdateDocuments_UpdateDocuments_CalledOnce()
        {
            _imanageRepository.UpdateDocuments(_imanageSetDocumentsPropertiesInput);

            Mock.Get(_mockImanageRest).Verify(
                i => i.UpdateDocuments(
                    It.IsAny<ImanageSetDocumentsPropertiesInput>()
                ), Times.Once);
        }
    }
}
