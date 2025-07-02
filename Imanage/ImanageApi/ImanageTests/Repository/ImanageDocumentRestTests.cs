using NUnit.Framework;
using RestSharp.Serialization.Json;
using System;

namespace Imanage.Tests
{
    [TestFixture()]
    public class ImanageDocumentRestTests
    {
        const string AUTHOR = "Author";
        const string OPERATOR = "Operator";
        const string CLASS = "Class";
        const string TYPE = "Type";
        const string FROM = "From";
        const string TO = "To";
        const string CC = "Cc";
        const string SUBJECT = "Subject";
        const string COMMENT = "Comment";
        const string DESCRIPTION = "Description";
        const string NUMBER = "Number";
        const string VERSION = "Version";

        DateTime _sentDate;
        DateTime _receivedDate;
        JsonSerializer _serializer;

        public ImanageDocumentRestTests()
        {
            _serializer = new JsonSerializer();
        }

        [SetUp()]
        public void Setup()
        {
            _sentDate = DateTime.Now;
            _receivedDate = DateTime.Now;
        }

        //[Test()]
        //public void ImanageDocumentRest_GetDocProfileEmail_AreEqualTest()
        //{
            
        //    var imanageDocumentRest = new ImanageDocumentRest()
        //    {
        //        Author = AUTHOR,
        //        Operator = OPERATOR,
        //        Class = CLASS,
        //        Type = TYPE,
        //        Name = DESCRIPTION,
        //        Comment = COMMENT,
        //        EmailProfileItems = new Imanage.Documents.EmailProfileItems(TO, FROM, CC, _sentDate, _receivedDate, SUBJECT)
        //    };

        //    var json = _serializer.Serialize("");
        //    var sentDate = _serializer.Serialize(_sentDate);
        //    var receivedDate = _serializer.Serialize(_receivedDate);

        //    Assert.AreEqual(
        //        "{\"From\":\"" + FROM 
        //        + "\",\"To\":\"" + TO
        //        + "\",\"Cc\":\"" + CC 
        //        + "\",\"SentDate\":" + sentDate
        //        + ",\"ReceivedDate\":" + receivedDate
        //        + ",\"Author\":\"" + AUTHOR
        //        + "\",\"Operator\":\"" + OPERATOR
        //        + "\",\"Class\":\"" + CLASS
        //        + "\",\"Type\":\"" + TYPE
        //        + "\",\"Comment\":\"" + COMMENT
        //        + "\",\"Name\":\"" + DESCRIPTION
        //        + "}", json);
        //}

        //[Test()]
        //public void ImanageDocumentRest_GetDocProfile_AreEqualTest()
        //{
        //    var imanageDocumentRest = new ImanageDocumentRest()
        //    {
        //        Author = AUTHOR,
        //        Operator = OPERATOR,
        //        Class = CLASS,
        //        Type = TYPE,
        //        Number = NUMBER,
        //        Version = VERSION,
        //        Name = DESCRIPTION,
        //        Comment = COMMENT,
        //        EmailProfileItems = null
        //    };

        //    var docProfile = imanageDocumentRest;
        //    var json = _serializer.Serialize(docProfile);

        //    Assert.AreEqual("{\"Author\":\"" + AUTHOR 
        //        + "\",\"Operator\":\"" + OPERATOR 
        //        + "\",\"Class\":\"" + CLASS 
        //        + "\",\"Type\":\"" + TYPE
        //        + "\",\"Comment\":" + COMMENT
        //        + ",\"Name\":" + DESCRIPTION 
        //        + ",\"Number\":" + NUMBER
        //        + ",\"Version\":" + VERSION + "}", json);

        //}

        //[Test()]
        //public void ImanageDocumentRest_GetDocProfileNumberVersion_AreEqualTest()
        //{

        //    var imanageDocumentRest = new ImanageDocumentRest()
        //    {
        //        Author = AUTHOR,
        //        Operator = OPERATOR,
        //        Class = CLASS,
        //        Type = TYPE,
        //        Number = NUMBER,
        //        Version = VERSION,
        //        Name = DESCRIPTION,
        //        Comment = COMMENT,
        //        EmailProfileItems = new Imanage.Documents.EmailProfileItems(TO, FROM, CC, _sentDate, _receivedDate, SUBJECT)
        //    };

        //    var docProfile = imanageDocumentRest;
        //    var json = _serializer.Serialize(docProfile);
        //    var sentDate = _serializer.Serialize(_sentDate);
        //    var receivedDate = _serializer.Serialize(_receivedDate);

        //    Assert.AreEqual(
        //        "{\"From\":\"" + FROM
        //        + "\",\"To\":\"" + TO
        //        + "\",\"Cc\":\"" + CC
        //        + "\",\"SentDate\":" + sentDate
        //        + ",\"ReceivedDate\":" + receivedDate
        //        + ",\"Author\":\"" + AUTHOR
        //        + "\",\"Operator\":\"" + OPERATOR
        //        + "\",\"Class\":\"" + CLASS
        //        + "\",\"Type\":\"" + TYPE
        //        + "\",\"Comment\":\"" + COMMENT
        //        + "\",\"Name\":\"" + DESCRIPTION
        //        + "\",\"Number\":\"" + NUMBER
        //        + "\",\"Version\":\"" + VERSION
        //        + "}", json);
        //    Assert.AreEqual(AUTHOR, docProfile.Author);
        //    Assert.AreEqual(OPERATOR, docProfile.Operator);
        //    Assert.AreEqual(CLASS, docProfile.Class);
        //    Assert.AreEqual(TYPE, docProfile.Type);
        //    Assert.AreEqual(FROM, docProfile.From);
        //    Assert.AreEqual(TO, docProfile.To);
        //    Assert.AreEqual(CC, docProfile.Cc);
        //    Assert.AreEqual(_sentDate, docProfile.SentDate);
        //    Assert.AreEqual(_receivedDate, docProfile.ReceivedDate);
        //}
    }
}
