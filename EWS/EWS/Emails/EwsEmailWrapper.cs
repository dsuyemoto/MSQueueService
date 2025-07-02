using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace EWS
{
    public partial class EwsWrapper
    {
        public IEnumerable<ExchEmail> GetEmails(ExchFolder exchangeFolder, int maxResults = 1000)
        {
            var props = new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.MimeContent, ItemSchema.Attachments);
            var view = new ItemView(maxResults);
            var items = _exchangeService.FindItems(new FolderId(exchangeFolder.UniqueId), view).Items;
            var emailList = new List<ExchEmail>();
            
            foreach (var item in items)
            {
                if (item is EmailMessage)
                {
                    var emailMessage = EmailMessage.Bind(_exchangeService, item.Id, props);
                    emailList.Add(new ExchEmail(emailMessage, false));
                }              
            }

            return emailList;
        }

        public IEnumerable<ExchEmail> GetEmails(ExchFolder exchangeFolder, ExchSearchFilter exchangeSearchFilter, int maxResults = 1000)
        {
            var props = new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.MimeContent, ItemSchema.Attachments);
            var view = new ItemView(maxResults);
            var items = _exchangeService.FindItems(new FolderId(exchangeFolder.UniqueId), exchangeSearchFilter.Filter, view).Items;
            var emailList = new List<ExchEmail>();

            foreach (var item in items)
            {
                if (item is EmailMessage)
                {
                    var emailMessage = EmailMessage.Bind(_exchangeService, item.Id, props);
                    emailList.Add(new ExchEmail(emailMessage, false));
                }
            }

            return emailList;
        }

        public ExchEmail GetEmail(string uniqueId)
        {
            var props = new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.MimeContent, ItemSchema.Attachments);

            var emailMessage = EmailMessage.Bind(_exchangeService, new ItemId(uniqueId), props);

            return new ExchEmail(emailMessage, false);
        }
    }
}
