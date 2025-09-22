namespace X.Neurons.P.LeoLedtech.Test.Server.Models.Authorize
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
