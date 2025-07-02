using LoggerHelper;
using ServiceNow;
using System;
using System.Collections.Generic;

namespace ServiceNowIntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Enabled = true;
            Logger.LoggerType = LoggerBase.LoggerType.Nlog;
            Logger.LogOutputs = new LogOutputBase[] {
                new ConsoleLogOutput(LoggerBase.LoggerLevel.Debug)
            };

            const string INCIDENTSYSID = "788fd4bedcb2d010b813b31551348d0d";
            const string USERSYSID = "254ad6514655676b01c5c6136a3a10fc";

            var snFields = new Dictionary<string, string>();
            var snRepo = new ServiceNowRepository();

            snFields.Add("sys_id", USERSYSID);
            var userResults = snRepo.GetUser(
                new SnUserGet(
                    args[0],
                    args[1],
                    args[2],
                    snFields,
                    null)
                );
            if (userResults.Error != null)
                Logger.Debug(userResults.Error);
            foreach (var result in userResults.SnFieldsList)
                foreach (var pair in result)
                    Logger.Debug("[" + USERSYSID + "]" + pair.Key + ": " + pair.Value);

            //var ticketResults = (List<SnTicketResult>)snRepo.GetTicket(
            //    new SnTicketGet(
            //        "incident",
            //        args[0],
            //        args[1],
            //        args[2],
            //        "4b9ab143d96810108ef19ac9eec833a8",
            //        new string[] { "number" }
            //        ));
            //foreach (var ticketResult in ticketResults)
            //    foreach (var pair in ticketResult.SnFields)
            //        Logger.Debug(pair.Key + ": " + pair.Value);

            //var ticketResults2 = snRepo.GetTicket(
            //    new SnTicketGet(
            //        "incident",
            //        args[0],
            //        args[1],
            //        args[2],
            //        INCIDENTSYSID,
            //        new string[] { "number", "assignment_group" }
            //        ));
            //foreach (var fieldsList in ticketResults2.SnFieldsList)
            //    foreach (var pair in fieldsList)
            //        Logger.Debug("[" + INCIDENTSYSID + "]" + pair.Key + ": " + pair.Value);

            snFields.Clear();
            snFields.Add("sys_id", INCIDENTSYSID);
            var ticketResults3 = snRepo.GetTicket(
                new SnTicketGet(
                    "incident",
                    args[0],
                    args[1],
                    args[2],
                    snFields,
                    new string[] { "number", "assignment_group", "sys_id" }
                    ));
            if (ticketResults3.Error != null)
                Logger.Debug(ticketResults3.Error);
            foreach (var fieldsList in ticketResults3.SnFieldsList)
                foreach (var pair in fieldsList)
                    Logger.Debug("[" + INCIDENTSYSID + "]" + pair.Key + ": " + pair.Value);
            Console.ReadKey();
        }
    }
}
