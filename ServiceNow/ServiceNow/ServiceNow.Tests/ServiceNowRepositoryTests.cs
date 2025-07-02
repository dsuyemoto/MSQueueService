using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using static ServiceNow.ServiceNowBase;

namespace ServiceNow.Tests
{
    [TestFixture()]
    public class ServiceNowRepositoryTests
    {
        IServiceNowRest _mockServiceNowRest;
        SnResultTable _snResultTableCreated;
        SnResultTable _snResultTableOk;
        SnResultTable _snResultTableNoContent;
        byte[] _content;
        Dictionary<string, object> _result;

        const string INSTANCEURL = "https://lathamtmp.service-now.com/";
        const string SYSID = "SysId";
        const string NUMBER = "number";
        List<Dictionary<string, object>> _results = new List<Dictionary<string, object>>();
        ServiceNowRepository _serviceNowRepository;
        const string USERNAME = "username";
        const string PASSWORD = "password";
        const string INCIDENT = "incident";
        const string FILENAME = "filename";
        const string MIMETYPE = "mimetype";
        const string TABLENAME = "Tablename";
        const string ERRORMESSAGE = "ErrorMessage";
        const string OUTLOOKNAME = "OutlookName";
        const string EMAILADDRESS = "EmailAddress";
        const string PHONE = "Phone";
        const string STATUSCODEOK = "OK";
        const string STATUSCODECREATED = "Created";
        const string STATUSCODENOCONTENT = "NoContent";

        public ServiceNowRepositoryTests()
        {
            _content = new byte[1];
            _result = new Dictionary<string, object>();
            _result.Add(SnField.sys_id.ToString(), SYSID);
            _result.Add(SnField.number.ToString(), NUMBER);
            _result.Add(SnField.email.ToString(), EMAILADDRESS);
            _result.Add(SnField.u_outlook_name.ToString(), OUTLOOKNAME);
            _result.Add(SnField.phone.ToString(), PHONE);
            _result.Add(SnField.user_name.ToString(), USERNAME);
            _snResultTableCreated = new SnResultTable(
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(new SnResponseTableDTO()
                    {
                        Result = _result
                    }),
                    StatusCode = HttpStatusCode.Created,
                    ResponseStatus = ResponseStatus.Completed,
                    ErrorException = new System.Exception(ERRORMESSAGE)
                },
                INSTANCEURL,
                TABLENAME);
            _snResultTableOk = new SnResultTable(
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(new SnResponseTableDTO()
                    {
                        Result = _result
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed
                },
                INSTANCEURL,
                TABLENAME);
            _snResultTableNoContent = new SnResultTable(
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(new SnResponseTableDTO()
                    {
                        Result = _result
                    }),
                    StatusCode = HttpStatusCode.NoContent,
                    ResponseStatus = ResponseStatus.Completed
                }, 
                INSTANCEURL,
                TABLENAME);
        }      

        [SetUp()]
        public void Setup()
        {
            _results.Clear();
            _results.Add(_result);
            _mockServiceNowRest = Mock.Of<IServiceNowRest>();
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.CreateAttachment(It.IsAny<SnAttachmentCreate>()))
                .Returns(_snResultTableCreated);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.CreateTicket(It.IsAny<SnTicketCreate>()))
                .Returns(_snResultTableCreated);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.GetTicket(It.IsAny<SnTicketGet>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.UpdateTicket(It.IsAny<SnTicketUpdate>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.GetUser(It.IsAny<SnUserGet>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.GetGroup(It.IsAny<SnGroupGet>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.DeleteTicket(It.IsAny<SnTicketDelete>()))
                .Returns(_snResultTableNoContent);
            Mock.Get(_mockServiceNowRest)
                .Setup(r => r.DeleteAttachment(It.IsAny<SnAttachmentDelete>()))
                .Returns(_snResultTableNoContent);
            _serviceNowRepository = new ServiceNowRepository(_mockServiceNowRest);
        }

        [Test()]
        public void CreateTicket_Result_AreEqualTest()
        {
            var ticket = new SnTicketCreate(
                INCIDENT,
                INSTANCEURL,
                USERNAME, 
                PASSWORD,
                new Dictionary<string, string>());

            var result = _serviceNowRepository.CreateTicket(ticket);
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(NUMBER, resultFields[SnField.number.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODECREATED, result.StatusCode);
        }

        [Test()]
        public void GetTicket_Number_AreEqualTest()
        {
            var ticket = new SnTicketGet(
                INCIDENT, 
                INSTANCEURL, 
                USERNAME,
                PASSWORD,
                new Dictionary<string, string>(), 
                null,
                SYSID);

            var result = _serviceNowRepository.GetTicket(ticket);
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(NUMBER, resultFields[SnField.number.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODEOK, result.StatusCode);
        }

        [Test()]
        public void UpdateTicket_SnTicketOutput_AreEqualTest()
        {
            var ticket = new SnTicketUpdate(
                INCIDENT, 
                INSTANCEURL, 
                USERNAME, 
                PASSWORD, 
                SYSID, 
                new Dictionary<string, string>());

            var result = _serviceNowRepository.UpdateTicket(ticket);
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODEOK, result.StatusCode);
        }

        [Test()]
        public void DeleteTicket_SnResult_AreEqualTest()
        {
            var ticket = new SnTicketDelete(INCIDENT, INSTANCEURL, USERNAME, PASSWORD, SYSID);

            var result = _serviceNowRepository.DeleteTicket(ticket);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODENOCONTENT, result.StatusCode);
        }

        [Test()]
        public void CreateAttachment_Results_AreEqualTest()
        {
            var attachment = new SnAttachmentCreate(
                INCIDENT, 
                INSTANCEURL,
                USERNAME, 
                PASSWORD,
                FILENAME, 
                MIMETYPE,
                SYSID,
                _content);

            var result = _serviceNowRepository.CreateAttachment(attachment);
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(ERRORMESSAGE, result.Error.Message);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.AreEqual(STATUSCODECREATED, result.StatusCode);
        }

        [Test()]
        public void DeleteAttachment_SnResult_AreEqualTest()
        {
            var attachment = new SnAttachmentDelete(INSTANCEURL, USERNAME, PASSWORD, SYSID);

            var result = _serviceNowRepository.DeleteAttachment(attachment);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODENOCONTENT, result.StatusCode);
        }

        [Test()]
        public void GetUser_SnResult_AreEqualTest()
        {
            var user = new SnUserGet(INSTANCEURL, USERNAME, PASSWORD, new Dictionary<string, string>(), null, SYSID);

            var result = _serviceNowRepository.GetUser(user);
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(EMAILADDRESS, resultFields[SnField.email.ToString()]);
            Assert.AreEqual(OUTLOOKNAME, resultFields[SnField.u_outlook_name.ToString()]);
            Assert.AreEqual(PHONE, resultFields[SnField.phone.ToString()]);
            Assert.AreEqual(USERNAME, resultFields[SnField.user_name.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODEOK, result.StatusCode);
        }

        [Test()]
        public void GetGroup_SnResult_AreEqualTest()
        {
            var group = new SnGroupGet(INSTANCEURL, USERNAME, PASSWORD, new Dictionary<string, string>(), null, SYSID);

            var result = _serviceNowRepository.GetGroup(group);
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(STATUSCODEOK, result.StatusCode);
        }
    }
}
