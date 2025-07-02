using Imanage.Documents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static Imanage.DocumentProfileItems;

namespace Imanage.Tests
{
    [TestFixture()]
    public class ImanageHelpersTests
    {
        const string DATABASE = "database";
        const string SESSION = "session";
        const string AUTHOR = "author";
        const string NUMBER = "007";
        const string VERSION = "1";
        const string DESCRIPTION = "description";
        const string COMMENT = "comment";
        const string OPERATOR = "operator";
        const string CLASS = "class";
        const string TYPE = "type";
        const string FROM = "From";
        const string TONAMES = "ToNames";
        const string CCNAMES = "CcNames";
        const string EXTENSION = "EML";

        DocumentProfileItems _documentProfileItems;
        DateTime _received = DateTime.Now;
        DateTime _sent = DateTime.Now;

        [SetUp()]
        public void Setup()
        {
            _documentProfileItems = new DocumentProfileItemsCreate(
            AUTHOR,
            COMMENT,
            DESCRIPTION,
            OPERATOR,
            EXTENSION,
            new EmailProfileItems(),
            true);
        }

        [Test()]
        public void GetDocNumberVersionTest()
        {
            string docFullId = @"!nrtdms:0:!session:gsoimandev1:!database:GSO:!document:18990301,1:";

            var docId = ImanageHelpers.GetDocNumberVersion(docFullId);

            Assert.AreEqual(new string[] { "18990301", "1" }, docId);
        }

        [Test()]
        public void ThrowImanageErrorTest()
        {
            var imanageObjectIds = new ImanageObjectId[] { };
            var errors = new ImanageError[] { new ImanageError("Import failed", null) };
            
            Assert.Throws<Exception>(() => ImanageHelpers.ThrowImanageError(imanageObjectIds, errors));
        }

        [Test()]
        public void CreateNrlLink_BlankFileName_AreEqualTest()
        {
            var nrlLink = ImanageHelpers.CreateNrlLink(new ImanageDocumentObjectId(SESSION, DATABASE, NUMBER, VERSION), "");

            Assert.AreEqual("blank.nrl", nrlLink.FileName);
        }

        [Test()]
        public void CreateNrlLink_FileName_AreEqualTest()
        {
            var fileName = "test filename";

            var nrlLink = ImanageHelpers.CreateNrlLink(new ImanageDocumentObjectId(SESSION, DATABASE, NUMBER, VERSION), fileName);

            Assert.AreEqual(fileName + ".nrl", nrlLink.FileName);
        }

        //[Test()]
        //public void BuildImProfileAttributeIds_Properties_IsTrueTest()
        //{
        //    var attributeIds = ImanageHelpers.BuildImProfileAttributeIds(_documentProfileItems);

        //    Assert.IsTrue(attributeIds.Contains(imProfileAttributeID.imProfileAuthor));
        //    Assert.IsTrue(attributeIds.Contains(imProfileAttributeID.imProfileComment));
        //    Assert.IsTrue(attributeIds.Contains(imProfileAttributeID.imProfileDescription));
        //    Assert.IsTrue(attributeIds.Contains(imProfileAttributeID.imProfileOperator));
        //    Assert.IsTrue(attributeIds.Contains(imProfileAttributeID.imProfileFrozen));
        //}

        [Test()]
        public void BuildProfileAttributeIdsFromDocumentProfileItems_Contains_IsTrueTest()
        {
            var attributeIds = ImanageHelpers.BuildProfileAttributeIds(_documentProfileItems);

            Assert.IsTrue(attributeIds.Contains(ProfileAttributeId.Author));
            Assert.IsTrue(attributeIds.Contains(ProfileAttributeId.Comment));
            Assert.IsTrue(attributeIds.Contains(ProfileAttributeId.Operator));
            Assert.IsTrue(attributeIds.Contains(ProfileAttributeId.Description));
        }

        //[Test()]
        //public void BuildEmailProfileItems_Properties_IsTrueTest()
        //{
        //    var profileItemsDict = new Dictionary<imProfileAttributeID, string>();
        //    _documentProfileItems.EmailProfileItems = new EmailProfileItems()
        //    {
        //        From = FROM,
        //        Received = _received,
        //        Sent = _sent,
        //        ToNames = TONAMES,
        //        CcNames = CCNAMES
        //    };
        //    var profileItems = ImanageHelpers.BuildEmailProfileItems(_documentProfileItems);
        //    foreach (var profileItem in profileItems)
        //        profileItemsDict.Add(profileItem.AttributeID, profileItem.Value);

        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom13));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom14));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom15));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom21));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom22));            
        //}

        //[Test()]
        //public void BuildProfileItems_EmailProperties_AreEqualTest()
        //{
        //    var profileItemsDict = new Dictionary<imProfileAttributeID, string>();
        //    _documentProfileItems.EmailProfileItems = new EmailProfileItems()
        //    {
        //        From = FROM,
        //        Received = _received,
        //        Sent = _sent,
        //        ToNames = TONAMES,
        //        CcNames = CCNAMES
        //    };

        //    var profileItems = ImanageHelpers.BuildProfileItems(_documentProfileItems);
        //    foreach (var profileItem in profileItems)
        //        profileItemsDict.Add(profileItem.AttributeID, profileItem.Value);

        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom13));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom14));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom15));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom21));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileCustom22));
        //}

        //[Test()]
        //public void BuildProfileItems_Keys_IsTrueTest()
        //{
        //    var profileItemsDict = new Dictionary<imProfileAttributeID, string>();

        //    var profileItems = ImanageHelpers.BuildProfileItems(
        //        new DocumentProfileItemsOutput() 
        //        { 
        //            Author = AUTHOR,
        //            Comment = COMMENT,
        //            Description = DESCRIPTION,
        //            Frozen = "False",
        //            Operator = OPERATOR
        //        });
        //    foreach (var profileItem in profileItems)
        //        profileItemsDict.Add(profileItem.AttributeID, profileItem.Value);

        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileAuthor));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileComment));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileDescription));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileOperator));
        //    Assert.IsTrue(profileItemsDict.ContainsKey(imProfileAttributeID.imProfileFrozen));
        //    Assert.AreEqual(AUTHOR, profileItemsDict[imProfileAttributeID.imProfileAuthor]);
        //    Assert.AreEqual(COMMENT, profileItemsDict[imProfileAttributeID.imProfileComment]);
        //    Assert.AreEqual(DESCRIPTION, profileItemsDict[imProfileAttributeID.imProfileDescription]);
        //    Assert.AreEqual(OPERATOR, profileItemsDict[imProfileAttributeID.imProfileOperator]);
        //    Assert.AreEqual("False", profileItemsDict[imProfileAttributeID.imProfileFrozen]);   
        //}

        [Test()]
        public void BuildProfileAttributeIdsForProfileAttributeIds_Contains_IsTrueTest()
        {
            var outputProfileIds = new string[] { "frozen", "MarkedForArchive" };

            var profileAttributeIds = ImanageHelpers.BuildProfileAttributeIds(outputProfileIds);
            var profileAttributeIdsList = new List<ProfileAttributeId>(profileAttributeIds);

            Assert.IsTrue(profileAttributeIdsList.Contains(ProfileAttributeId.MarkedForArchive));
            Assert.IsTrue(profileAttributeIdsList.Contains(ProfileAttributeId.Frozen)); 
        }
    }
}