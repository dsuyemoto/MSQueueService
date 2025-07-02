using PureConnect;
using System;
using System.Collections.Generic;
using System.Net;

namespace PureConnect.Integration.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //const string EICACCOUNTCODE = "Eic_AccountCode";
                //var ininService = new ININService();
                //var attributes = new Dictionary<string, string>();
                //attributes.Add("Eic_Note", "Test ICWS2");
                //var interactions = new ININInteractionsResource(ininService);
                //interactions.Connect("GSOCICDEV", "dsuyemot", "gtsc");
                //var updateresult = interactions.UpdateInteractionAttributesAsync(
                //    new ININUpdateInteractionAttributesRequestParameters() { InteractionId = "1001794706" },
                //    new ININInteractionAttributesUpdateDataContract() { Attributes = attributes }
                //    ).Result;

                //if (updateresult.IsSuccessful)
                //    Console.WriteLine("Update was successful!");
                //Console.WriteLine(updateresult.StatusCode);

                //var getresult = interactions.GetInteractionAttributesAsync(
                //    new ININGetInteractionAttributesRequestParameters()
                //    {
                //        InteractionId = "1001794706",
                //        Select = EICACCOUNTCODE
                //    }).Result;

                //if (getresult.IsSuccessful)
                //    if (getresult.InteractionAttributesDataContract.Attributes.ContainsKey(EICACCOUNTCODE))
                //        Console.WriteLine(getresult.InteractionAttributesDataContract.Attributes[EICACCOUNTCODE]);

                var demo = new Demo();
                demo.Execute();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.Read();

        }

        private static void LogError(HttpStatusCode status)
        {
            Console.WriteLine(status);
        }
    }
}
