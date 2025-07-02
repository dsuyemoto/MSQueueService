using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using static ServiceNow.ServiceNowBase;

namespace ServiceNow.Tests
{
    [TestFixture()]
    public class ServiceNowRestTests
    {
        ServiceNowRest _serviceNowRest;
        IRestClient _mockRestClient;
        byte[] _content;
        string _responseJSON;

        const string TABLENAME = "Tablename";
        const string INSTANCEURL = "InstanceUrl";
        const string USERNAME = "Username";
        const string PASSWORD = "Password";
        const string FILENAME = "Filename";
        const string MIMETYPE = "image/jpeg";
        const string SYSID = "994adbc64f511200adf9f8e18110c796";
        const string TABLESYSID = "d71f7935c0a8016700802b64c67c11c6";
        const string NUMBER = "INC123456";
        const string EMAILADDRESS = "douglas.suyemoto@lw.com";

        public ServiceNowRestTests()
        {
            _content = new byte[1];
            _responseJSON = @"{
              'result': {
                'table_sys_id': '[TABLESYSID]',
                'size_bytes': '36597',
                'download_link': 'https://instance.service-now.com/api/now/attachment/994adbc64f511200adf9f8e18110c796/file',
                'sys_updated_on': '2016-02-02 14:00:21',
                'sys_id': '[SYSID]',
                'image_height': '',
                'sys_created_on': '2016-02-02 14:00:21',
                'file_name': 'banner-CS0001345_v1_1.jpeg',
                'sys_created_by': 'admin',
                'compressed': 'true',
                'average_image_color': '',
                'sys_updated_by': 'admin',
                'sys_tags': '',
                'table_name': 'incident',
                'image_width': '',
                'sys_mod_count': '0',
                'content_type': '[CONTENTTYPE]',
                'size_compressed': '25130',
                'number': '[NUMBER]',
                'email': '[EMAILADDRESS]'
              }
            }";
            _responseJSON = _responseJSON
                .Replace("[TABLESYSID]", TABLESYSID)
                .Replace("[SYSID]", SYSID)
                .Replace("[CONTENTTYPE]", MIMETYPE)
                .Replace("[NUMBER]", NUMBER)
                .Replace("[EMAILADDRESS]", EMAILADDRESS);

        }

        [SetUp]
        public void Setup()
        {
            _mockRestClient = Mock.Of<IRestClient>();
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse() { 
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);
        }

        [Test()]
        public void CreateAttachment_Results_AreEqualTest()
        {
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.Created,
                    ResponseStatus = ResponseStatus.Completed
                });

            var result = _serviceNowRest.CreateAttachment(
                new SnAttachmentCreate(
                    TABLENAME, 
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    FILENAME,
                    MIMETYPE,
                    SYSID,
                    _content));
            var snFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, snFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(TABLESYSID, snFields[SnField.table_sys_id.ToString()]);
            Assert.AreEqual(MIMETYPE, snFields[SnField.content_type.ToString()]);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.AreEqual(TABLENAME, result.TableName);
        }

        [Test]
        public void DeleteAttachment_Results_AreEqualTest()
        {
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.NoContent,
                    ResponseStatus = ResponseStatus.Completed
                });

            var result = _serviceNowRest.DeleteAttachment(
                new SnAttachmentDelete(
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    SYSID));

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.IsNull(result.TableName);
        }

        [Test]
        public void CreateTicket_Results_AreEqualTest()
        {
            _mockRestClient = Mock.Of<IRestClient>();
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.Created,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);

            var snFields = new Dictionary<string, string>();
            snFields.Add(SnField.assignment_group.ToString(), "AssignedGroup");
            var jsonBody = JsonConvert.SerializeObject(snFields);
            var result = _serviceNowRest.CreateTicket(
                new SnTicketCreate(
                    TABLENAME,
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    snFields));
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(NUMBER, resultFields[SnField.number.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.AreEqual(TABLENAME, result.TableName);
        }

        [Test]
        public void UpdateTicket_Results_AreEqualTest()
        {
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);

            var snFields = new Dictionary<string, string>();
            snFields.Add("u_cic_id", "123456");
            var result = _serviceNowRest.UpdateTicket(
                new SnTicketUpdate(
                    TABLENAME,
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    SYSID,
                    snFields));
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(NUMBER, resultFields[SnField.number.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.AreEqual(TABLENAME, result.TableName);
        }

        [Test]
        public void GetTicket_Results_AreEqualTest()
        {
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);

            var snFields = new Dictionary<string, string>();
            snFields.Add(SnField.sys_id.ToString(), SYSID);
            var result = _serviceNowRest.GetTicket(
                new SnTicketGet(
                    TABLENAME,
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    snFields,
                    new string[] { SnField.sys_id.ToString(), SnField.number.ToString() },
                    SYSID));
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(NUMBER, resultFields[SnField.number.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.AreEqual(TABLENAME, result.TableName);
        }

        [Test]
        public void DeleteTicket_Results_AreEqualTest()
        {
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.NoContent,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);

            var snFields = new Dictionary<string, string>();
            snFields.Add(SnField.sys_id.ToString(), SYSID);
            var result = _serviceNowRest.DeleteTicket(
                new SnTicketDelete(
                    TABLENAME,
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    SYSID));

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.AreEqual(TABLENAME, result.TableName);
        }

        [Test]
        public void GetUser_Results_AreEqualTest()
        {
            _mockRestClient = Mock.Of<IRestClient>();
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);

            var snFields = new Dictionary<string, string>();
            snFields.Add(SnField.sys_id.ToString(), SYSID);
            var result = _serviceNowRest.GetUser(
                new SnUserGet(
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    snFields,
                    new string[] { SnField.sys_id.ToString(), SnField.number.ToString() },
                    SYSID));
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.AreEqual(EMAILADDRESS, resultFields[SnField.email.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.IsNull(result.TableName);
        }

        [Test]
        public void GetGroup_Results_AreEqualTest()
        {
            _mockRestClient = Mock.Of<IRestClient>();
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse()
                {
                    Content = _responseJSON,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed
                });
            _serviceNowRest = new ServiceNowRest(_mockRestClient);

            var snFields = new Dictionary<string, string>();
            snFields.Add(SnField.sys_id.ToString(), SYSID);
            var result = _serviceNowRest.GetGroup(
                new SnGroupGet(
                    INSTANCEURL,
                    USERNAME,
                    PASSWORD,
                    snFields,
                    new string[] { SnField.sys_id.ToString(), SnField.number.ToString() },
                    SYSID));
            var resultFields = ((SnResultTable)result).SnFields;

            Assert.AreEqual(SYSID, resultFields[SnField.sys_id.ToString()]);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(INSTANCEURL, result.InstanceUrl);
            Assert.IsNull(result.TableName);
        }

        [Test]
        public void TestDeserialization()
        {
            var response = "{ \r\n  \"result\": {\r\n    \"table_sys_id\" : \"testsysid\"  \r\n}\r\n}";
            var responseCreateTicketDTO = JsonConvert.DeserializeObject<SnResponseTableDTO>(response);
            Assert.AreEqual("testsysid", responseCreateTicketDTO.Result["table_sys_id"]);
        }
    }
}