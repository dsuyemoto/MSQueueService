namespace PureConnect
{
    public class ININServerCredentials
    {
        public string Server { get; }
        public string Username { get; }
        public string Password { get; }

        public ININServerCredentials(string server, string username, string password)
        {
            Server = server;
            Username = username;
            Password = password;
        }
    }
}
