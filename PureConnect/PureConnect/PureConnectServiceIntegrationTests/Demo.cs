using ININ.ICWS;
using ININ.ICWS.Configuration.People;
using ININ.ICWS.Connection;
using ININ.WebServices.Core;
using System;
using static ININ.ICWS.Connection.ConnectionResource;

namespace PureConnect.Integration.Tests
{
    public class Demo
    {
        public Demo()
        {
            
        }

        public void Execute()
        {
            // Configures web service utility
            WebServiceUtility webServiceUtility = new WebServiceUtility()
            {
                Port = 8018,
                IsHttps = false,
                Server = "name_of_server"
            };
            Console.WriteLine(webServiceUtility.Port);

            //Initializes/authenticates web service utility with CIC server
            var result = new ConnectionResource(webServiceUtility).CreateConnection(
                    new CreateConnectionRequestParameters()
                    {
                        Accept_Language = "en-us",
                        Include = "features"
                    },
                    new IcAuthConnectionRequestSettingsDataContract()
                    {
                        UserID = "cic_user_id",
                        Password = "cic_user_password",
                        ApplicationName = "PureConnectService",
                    }).Result;
            Console.WriteLine(result.ININ_ICWS_Session_ID);

            //Retrieves response from CIC server and caches token/cookie for web service utility
            result.PerformIfResponseIs201(response => {
                webServiceUtility.SessionParameters =
                new AuthenticationParameters
                {
                    SessionId = response.SessionId,
                    Cookie = result.Set_Cookie,
                    ININ_ICWS_CSRF_Token = response.CsrfToken
                };
                Console.WriteLine(result.StatusCode);
            });

            //Retrieves user using user ID and writes out email address of user
            UsersResource usersResource = new UsersResource(webServiceUtility);
            UsersResource.GetUserResponses userResult = usersResource.GetUser(new UsersResource.GetUserRequestParameters()
            {
                Id = "cic_user_id"
            }).Result;
            Console.WriteLine(userResult.StatusCode);

            userResult.PerformIfResponseIs200(u => {
                if (u.SkillsHasValue)
                    Console.WriteLine(u.Skills);
            });
        }
    }
}
