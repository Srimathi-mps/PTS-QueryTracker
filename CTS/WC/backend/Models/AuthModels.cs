namespace QueryTracker.Api.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string UserName { get; set; } = string.Empty;
    }
}
