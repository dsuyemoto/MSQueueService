using EWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace Ews.IntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Params: {0}, {1}, {2}, {3}", args[0], args[1], args[2], args[3]);

                var mailboxEmailAddress = args[0];
                var networkUsername = args[1];
                var networkPassword = args[2];
                var exchangeFolder = args[3];
                
                var credentials = new NetworkCredential(networkUsername, networkPassword);

                var ewsWrapper = new EwsWrapper(mailboxEmailAddress, credentials);
                var exchSearchFilter = new ExchSearchFilter();
                exchSearchFilter.ContainsSubstring(FolderSchema.DisplayName, exchangeFolder, ContainmentMode.FullString, ComparisonMode.IgnoreCase);
                var foundFolder = (List<ExchFolder>)ewsWrapper.FindFolders(ewsWrapper.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot), exchSearchFilter);

                var output = foundFolder != null ? foundFolder[0].Name : "No";
                Console.WriteLine("{0} Folder Found", output);

                var searchFilter = new ExchSearchFilter();
                searchFilter.ContainsSubstring(ItemSchema.Subject, "Spectre and Meltdown Vulnerabilities", ContainmentMode.Substring, ComparisonMode.IgnoreCase);
                var emails = (List<ExchEmail>)ewsWrapper.GetEmails(foundFolder[0], searchFilter);

                if (emails.Count() > 0)
                    Console.WriteLine("First Email Subject: {0}", emails[0].Subject); 

            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Microsoft.Exchange.WebServices.Autodiscover.AutodiscoverRemoteException ex)
            {
                Console.WriteLine(ex.Error.Message);
            }
            finally
            {
                Console.Read();
            }
        }
    }
}
