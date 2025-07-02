namespace MSMQHandlerService.Models
{
    public class ImanageDocumentSecurity
    {
        public string SecurityUsername { get; set; }
        public string SecurityPasswordBase64 { get; set; }
        public string Session { get; set; }
        public string Database { get; set; }

        public ImanageDocumentSecurity(string username, string passwordBase64, string session, string database)
        {
            SecurityUsername = username;
            SecurityPasswordBase64 = passwordBase64;
            Session = session;
            Database = database;
        }

        public ImanageDocumentSecurity()
        {

        }
    }
}