namespace rgnl_server.Models.Resources
{
    public class LoginResponseResource
    {
        public int Id { get; set; }
        public string AuthToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}