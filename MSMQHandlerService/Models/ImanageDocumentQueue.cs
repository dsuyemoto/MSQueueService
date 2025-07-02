using Imanage;
using System;
using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ImanageDocumentQueue : ImanageDocumentSecurity
    {
        public string Author { get; set; }
        public string Operator { get; set; }
        public string DescriptionBase64 { get; set; }
        public string CommentBase64 { get; set; }
        public ImanageEmailPropertiesQueue EmailProperties { get; set; }
        public string ContentBytesBase64 { get; set; }
        public string Number { get; set; }
        public string Version { get; set; }
        public ImanageDocumentNrlQueue ImanageNrl { get; set; }
        public ImanageErrorQueue ImanageError { get; set; }
        public string Extension { get; set; }

        public ImanageDocumentQueue()
        {

        }

        public ImanageDocumentQueue(ImanageDocumentOutput imanageDocumentOutput)
        {
            if (imanageDocumentOutput == null) return;

            var nrlName = "blank";

            if (imanageDocumentOutput.DocumentProfileItems != null)
            {
                Author = imanageDocumentOutput.DocumentProfileItems.Author;
                Operator = imanageDocumentOutput.DocumentProfileItems.Operator;
                DescriptionBase64 = Helpers.Base64ConvertTo(imanageDocumentOutput.DocumentProfileItems.Description);
                CommentBase64 = Helpers.Base64ConvertTo(imanageDocumentOutput.DocumentProfileItems.Comment);

                if (!string.IsNullOrEmpty(imanageDocumentOutput.DocumentProfileItems.Description))
                    nrlName = imanageDocumentOutput.DocumentProfileItems.Description;
                EmailProperties = new ImanageEmailPropertiesQueue(imanageDocumentOutput.DocumentProfileItems.EmailProfileItems);
                Extension = imanageDocumentOutput.DocumentProfileItems.Extension;
            }

            if (imanageDocumentOutput.DocumentObjectId != null)
            {
                Number = imanageDocumentOutput.DocumentObjectId.Number;
                Version = imanageDocumentOutput.DocumentObjectId.Version;
                ImanageNrl = new ImanageDocumentNrlQueue(
                    ImanageHelpers.CreateNrlLink(
                        imanageDocumentOutput.DocumentObjectId,
                        nrlName));
                Session = imanageDocumentOutput.DocumentObjectId.Session;
                Database = imanageDocumentOutput.DocumentObjectId.Database;
            };
            ContentBytesBase64 = Helpers.Base64ConvertTo(imanageDocumentOutput.Content);
            ImanageError = new ImanageErrorQueue(imanageDocumentOutput.ImanageError);
        }

        public ImanageDocumentQueue Copy()
        {
            return new ImanageDocumentQueue()
            {
                Author = Author,
                CommentBase64 = CommentBase64,
                ContentBytesBase64 = ContentBytesBase64,
                Database = Database,
                DescriptionBase64 = DescriptionBase64,
                EmailProperties = EmailProperties,
                Operator = Operator,
                SecurityPasswordBase64 = SecurityPasswordBase64,
                SecurityUsername = SecurityUsername,
                Session = Session,
                ImanageError = ImanageError,
                ImanageNrl = ImanageNrl,
                Extension = Extension
            };
        }
    }
}