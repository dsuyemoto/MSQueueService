namespace Imanage
{
    public class ImanageCreds
    {
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ImanageCreds(string database, string username, string password)
        {
            Database = database;
            Username = username;
            Password = password;
        }
    }
}
