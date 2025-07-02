using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace EWS.Tests
{
    [TestFixture()]
    public class ExchEmailTests
    {
        EwsWrapper _ewsWrapper;

        public ExchEmailTests()
        {
            var ewsWrapperConnector = new EwsWrapperConnector();
            _ewsWrapper = ewsWrapperConnector.Instance();
        }

        [Test()]
        public void ExchEmailParametersIntegrationsTest()
        {
            var msgRootFolder = _ewsWrapper.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot);
            var archiveFolder = _ewsWrapper.FindFolder(msgRootFolder, "ArchiveFolders");
            var integrationFolder = _ewsWrapper.FindFolder(archiveFolder, "Integration Testing");
            var testEmails = (List<ExchEmail>)_ewsWrapper.GetEmails(integrationFolder);
            var testEmail = testEmails[0];

            Assert.AreEqual("douglas.suyemoto@lw.com", testEmail.ToEmailAddresses[0].ToLower());
            Assert.AreEqual("Suyemoto, Douglas (GSO)", testEmail.ToNames);
            Assert.AreEqual("cic.gtsc.dev@lw.com", testEmail.CcEmailAddresses[0].ToLower());
            Assert.AreEqual("CIC - GTSC (DEV)", testEmail.CcNames);
            Assert.AreEqual("douglas.suyemoto@lw.com", testEmail.FromEmailAddress.ToLower());
            Assert.AreEqual("Suyemoto, Douglas (GSO)", testEmail.FromName);
            Assert.AreEqual("Test for integration testing", testEmail.Subject);
            Assert.AreEqual("<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\"" +
                " xmlns:w=\"urn:schemas-microsoft-com:office:word\" xmlns:m=\"http://schemas.microsoft.com/office/2004/12/omml\" " +
                "xmlns=\"http://www.w3.org/TR/REC-html40\">\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; " +
                "charset=utf-8\">\r\n<meta name=\"Generator\" content=\"Microsoft Word 15 (filtered medium)\">\r\n<style><!--\r\n/* " +
                "Font Definitions */\r\n@font-face\r\n\t{font-family:\"Cambria Math\";\r\n\tpanose-1:2 4 5 3 5 4 6 3 2 4;}\r\n@font-face\r\n\t" +
                "{font-family:Calibri;\r\n\tpanose-1:2 15 5 2 2 2 4 3 2 4;}\r\n/* Style Definitions */\r\np.MsoNormal, li.MsoNormal, " +
                "div.MsoNormal\r\n\t{margin:0in;\r\n\tmargin-bottom:.0001pt;\r\n\tfont-size:11.0pt;\r\n\tfont-family:\"Calibri\",sans-serif;}" +
                "\r\na:link, span.MsoHyperlink\r\n\t{mso-style-priority:99;\r\n\tcolor:#0563C1;\r\n\ttext-decoration:underline;}\r\na:visited, " +
                "span.MsoHyperlinkFollowed\r\n\t{mso-style-priority:99;\r\n\tcolor:#954F72;\r\n\ttext-decoration:underline;}\r\nspan.EmailStyle17" +
                "\r\n\t{mso-style-type:personal-compose;\r\n\tfont-family:\"Calibri\",sans-serif;\r\n\tcolor:windowtext;}\r\n.MsoChpDefault\r\n\t" +
                "{mso-style-type:export-only;\r\n\tfont-family:\"Calibri\",sans-serif;}\r\n@page WordSection1\r\n\t{size:8.5in 11.0in;\r\n\t" +
                "margin:1.0in 1.0in 1.0in 1.0in;}\r\ndiv.WordSection1\r\n\t{page:WordSection1;}\r\n--></style><!--[if gte mso 9]><xml>\r\n" +
                "<o:shapedefaults v:ext=\"edit\" spidmax=\"1026\" />\r\n</xml><![endif]--><!--[if gte mso 9]><xml>\r\n<o:shapelayout v:ext=\"edit\">" +
                "\r\n<o:idmap v:ext=\"edit\" data=\"1\" />\r\n</o:shapelayout></xml><![endif]-->\r\n</head>\r\n<body lang=\"EN-US\" link=\"#0563C1\" " +
                "vlink=\"#954F72\">\r\n<div class=\"WordSection1\">\r\n<p class=\"MsoNormal\"><o:p>&nbsp;</o:p></p>\r\n<p class=\"MsoNormal\"><o:p>&nbsp;" +
                "</o:p></p>\r\n<p class=\"MsoNormal\"><b><span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#010000\"" +
                ">Doug Suyemoto</span></b><span style=\"font-size:12.0pt;font-family:&quot;Times New Roman&quot;,serif\"><o:p></o:p></span></p>\r\n<p " +
                "class=\"MsoNormal\"><span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#010000\">Global Support Services Engineer" +
                "</span><span style=\"font-size:12.0pt;font-family:&quot;Times New Roman&quot;,serif\"><o:p></o:p></span></p>\r\n<p class=\"MsoNormal\">" +
                "<span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#010000\">&nbsp;</span><span style=\"font-size:12.0pt;" +
                "font-family:&quot;Times New Roman&quot;,serif\"><o:p></o:p></span></p>\r\n<p class=\"MsoNormal\"><b><span style=\"font-size:10.0pt;" +
                "font-family:&quot;Arial&quot;,sans-serif;color:maroon\">LATHAM</span></b><b><span style=\"font-size:7.5pt;font-family:&quot;Arial&quot;" +
                ",sans-serif;color:maroon\"> &amp;\r\n</span></b><b><span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:maroon\">" +
                "WATKINS</span></b><b><span style=\"font-size:7.5pt;font-family:&quot;Arial&quot;,sans-serif;color:maroon\"> LLP</span></b>" +
                "<span style=\"font-size:12.0pt;font-family:&quot;Times New Roman&quot;,serif\"><o:p></o:p></span></p>\r\n<p class=\"MsoNormal\">" +
                "<span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#010000\">" +
                "555 West Fifth Street, Suite 800 | Los Angeles, CA 90013-1021</span><span style=\"font-size:12.0pt;font-family:&quot;Times New Roman&quot;" +
                ",serif\"><o:p></o:p></span></p>\r\n<p class=\"MsoNormal\"><span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;" +
                "color:#010000\">D: &#43;1.213.892.3968 | M: &#43;1.310.619.1098</span><span style=\"font-size:12.0pt;font-family:&quot;Times New Roman&quot;" +
                ",serif\"><o:p></o:p></span></p>\r\n<p class=\"MsoNormal\"><span style=\"font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;" +
                "color:#010000\">&nbsp;</span><o:p></o:p></p>\r\n<p class=\"MsoNormal\"><o:p>&nbsp;</o:p></p>\r\n</div>\r\n</body>\r\n</html>\r\n", testEmail.Body);
            Assert.AreEqual("douglas.suyemoto@lw.com", testEmail.SenderEmailAddress.ToLower());
            Assert.AreEqual("Suyemoto, Douglas (GSO)", testEmail.SenderName);
            Assert.AreEqual(11917, testEmail.Content.Length);
            Assert.AreEqual("AQMkADk3M2U4NmE2LTZkYmUtNDBmMC1hNThhLTRiN2I3M2FkMWQxZQAuAAADSkXGee4uC0O0YAMr9tsG0QEAGFCgxcxa7UmG/3oRRHFbKQACPBPH6wAAAA==", testEmail.ParentFolderUniqueId);
            Assert.AreEqual("AAMkADk3M2U4NmE2LTZkYmUtNDBmMC1hNThhLTRiN2I3M2FkMWQxZQBGAAAAAABKRcZ57i4LQ7RgAyv22wbRBwAYUKDFzFrtSYb/ehFEcVspAAI8E8frAAAYUKDFzFrtSYb/ehFEcVspAAI8E/XIAAA=", testEmail.UniqueId);
            Assert.AreEqual(DateTime.FromBinary(-8586818337964775808), testEmail.Sent);
            Assert.AreEqual(DateTime.FromBinary(-8586818337954775808), testEmail.Received);
        }

        [Test()]
        public void ExchEmailAttachmentsIntegrationsTest()
        {
            var msgRootFolder = _ewsWrapper.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot);
            var archiveFolder = _ewsWrapper.FindFolder(msgRootFolder, "ArchiveFolders");
            var integrationFolder = _ewsWrapper.FindFolder(archiveFolder, "Integration Testing");
            var testEmails = (List<ExchEmail>)_ewsWrapper.GetEmails(integrationFolder);
            var testEmail = testEmails[0];
            var testAttachments = testEmail.Attachments;

            var testAttachment = testAttachments[0];

            Assert.IsNotNull(testAttachment.Content);
            Assert.IsNotNull(testAttachment.Email);
            Assert.IsNotNull(testAttachment.Name);
        }
    }
}