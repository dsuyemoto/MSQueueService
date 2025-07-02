using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;

namespace EWS
{
    public class ExchEmail : IExchItem
    {
        public byte[] Content { get; set; } = new Byte[0];
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromName { get; set; }
        public string FromEmailAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderEmailAddress { get; set; }
        public string ToNames { get; set; }
        public List<string> ToEmailAddresses { get; set; } = new List<string>();
        public string CcNames { get; set; }
        public List<string> CcEmailAddresses { get; set; } = new List<string>();
        public List<string> BccEmailAddresses { get; set; } = new List<string>();
        public DateTime Received { get; set; }
        public DateTime Sent { get; set; }
        public List<IExchAttachment> Attachments { get; set; } = new List<IExchAttachment>();
        public string UniqueId { get; set; }
        public string ParentFolderUniqueId { get; set; }
        public bool IsAttachment { get; private set; }

        public ExchEmail()
        {
        }

        public ExchEmail(EmailMessage emailMessage, bool isAttachment = false)
        {
            ToNames = emailMessage.DisplayTo;
            foreach (var recipient in emailMessage.ToRecipients)
                ToEmailAddresses.Add(recipient.Address);
            CcNames = emailMessage.DisplayCc;
            foreach (var recipient in emailMessage.CcRecipients)
                CcEmailAddresses.Add(recipient.Address);
            foreach (var recipient in emailMessage.BccRecipients)
                BccEmailAddresses.Add(recipient.Address);

            EmailAddress emailAddress;
            if (emailMessage.TryGetProperty(EmailMessageSchema.From, out emailAddress)) FromName = emailAddress.Name;
            if (emailMessage.TryGetProperty(EmailMessageSchema.From, out emailAddress)) FromEmailAddress = emailAddress.Address;
            if (emailMessage.TryGetProperty(EmailMessageSchema.Sender, out emailAddress)) SenderName = emailAddress.Name;
            if (emailMessage.TryGetProperty(EmailMessageSchema.Sender, out emailAddress)) SenderEmailAddress = emailAddress.Address;
            object output;
            if (emailMessage.TryGetProperty(ItemSchema.Subject, out output)) Subject = (string)output;
            if (emailMessage.TryGetProperty(ItemSchema.Body, out output)) Body = (MessageBody)output;
            if (emailMessage.TryGetProperty(ItemSchema.DateTimeSent, out output)) Sent = (DateTime)output;
            if (emailMessage.TryGetProperty(ItemSchema.DateTimeReceived, out output)) Received = (DateTime)output;
            if (!isAttachment)
            {
                UniqueId = emailMessage.Id.UniqueId;
                ParentFolderUniqueId = emailMessage.ParentFolderId.UniqueId;
            }
            IsAttachment = isAttachment;
            if (emailMessage.HasAttachments)
            {
                foreach (var attachment in emailMessage.Attachments)
                    if (attachment is FileAttachment)
                    {
                        var fileAttachment = attachment as FileAttachment;
                        fileAttachment.Load();
                        Attachments.Add(new ExchAttachmentFile(fileAttachment));
                    }
                    else if (attachment is ItemAttachment)
                    {
                        var itemAttachment = attachment as ItemAttachment;
                        var props = new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.MimeContent, ItemSchema.Attachments);
                        itemAttachment.Load(props);
                        if (itemAttachment.Item is EmailMessage)
                            Attachments.Add(new ExchAttachmentEmail(itemAttachment));
                    }
            }
            if (emailMessage.MimeContent != null)
                Content = emailMessage.MimeContent.Content;
        }
    }
}
