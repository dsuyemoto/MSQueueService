using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using System;

namespace Imanage
{
    public class ImanageHistoryItem
    {
        public string Application { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
        public string Number { get; set; }
        public string Operation { get; set; }
        public int PagesPrinted { get; set; }
        public string User { get; set; }
        public string Version { get; set; }

        public ImanageHistoryItem(HistoryItem historyitem)
        {
            Application = historyitem.Application;
            Comment = historyitem.Comment;
            Date = historyitem.Date;
            Duration = historyitem.Duration;
            Location = historyitem.Location;
            Number = historyitem.Number;
            Operation = historyitem.Operation;
            PagesPrinted = historyitem.PagesPrinted;
            User = historyitem.User;
            Version = historyitem.Version;
        }

        public HistoryItem GetHistoryItem()
        {
            var historyItem = new HistoryItem();
            historyItem.Application = Application;
            historyItem.Comment = Comment;
            historyItem.Date = Date;
            historyItem.Duration = Duration;
            historyItem.Location = Location;
            historyItem.Number = Number;
            historyItem.Operation = Operation;
            historyItem.PagesPrinted = PagesPrinted;
            historyItem.User = User;
            historyItem.Version = Version;
            return historyItem;
        }
    }
}
