using Imanage.Documents;
using NUnit.Framework;
using System;
using static Imanage.DocumentProfileItems;

namespace Imanage.Tests
{
    [TestFixture()]
    public class DocumentProfileItemsTests
    {
        const string DESCRIPTIONJPG = "description.jpg";
        const string DESCRIPTIONBAD = "description.badextension";
        const string AUTHOR = "Author";
        const string COMMENT = "Comment";
        const string DESCRIPTION = "Description";
        const string IMANOPERATOR = "Operator";
        const string EMAILTO = "To";
        const string EMAILCC = "Cc";
        const string EMAILFROM = "From"; 
        const string EMAILSUBJECT = "Subject";
        const string IMANCLASS = "Imanclass";
        const string IMANAPPTYPE = "Imanapptype";
        const string EXTENSION = "EML";
        
        DateTime _emailReceived = DateTime.Now;
        DateTime _emailSent = DateTime.Now;

        DocumentProfileItems _documentProfileItems;

        [SetUp()]
        public void Setup()
        {
            _documentProfileItems = new DocumentProfileItemsCreate(
                AUTHOR,
                COMMENT,
                DESCRIPTION,
                IMANOPERATOR,
                EXTENSION,
                new EmailProfileItems()
                );
        }

        [Test()]
        public void UpdateDescription_DescriptionLessThan254Characters_AreEqualTests()
        {
            _documentProfileItems.Description = DESCRIPTIONJPG;

            Assert.AreEqual(DESCRIPTIONJPG, _documentProfileItems.Description);
        }

        [Test()]
        public void UpdateDescription_DescriptionGreaterThan254Characters_AreEqualTests()
        {
            var description = new string('*', 255);

            _documentProfileItems.Description = description;

            Assert.AreEqual(description.Substring(0, 254), _documentProfileItems.Description);
        }

        [Test()]
        public void EmailProfileItems_EmailProperties_AreEqualEmail()
        {
            _documentProfileItems = new DocumentProfileItemsSet(
                AUTHOR,
                COMMENT,
                DESCRIPTION,
                IMANOPERATOR,
                new EmailProfileItems(EMAILTO, EMAILFROM, EMAILCC, _emailSent.ToString(), _emailReceived.ToString(), EMAILSUBJECT));

            Assert.AreEqual(EMAILFROM, _documentProfileItems.EmailProfileItems.From);
            Assert.AreEqual(EMAILTO, _documentProfileItems.EmailProfileItems.ToNames);
            Assert.AreEqual(EMAILCC, _documentProfileItems.EmailProfileItems.CcNames);
            Assert.AreEqual(EMAILSUBJECT, _documentProfileItems.EmailProfileItems.Subject);
            Assert.AreEqual(_emailReceived.ToString(), _documentProfileItems.EmailProfileItems.Received.ToString());
            Assert.AreEqual(_emailSent.ToString(), _documentProfileItems.EmailProfileItems.Sent.ToString());
        }
    }
}