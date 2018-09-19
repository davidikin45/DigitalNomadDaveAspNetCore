namespace DND.Common.Infrastructure.Settings
{
    public class LoginSettings
    {
        public bool AllowAnonymousUsers { get; set; }
        public Login Application { get; set; }
        public Login JwtToken { get; set; }
        public Login OpenIdConnect { get; set; }
        public Login OpenIdConnectJwtToken { get; set; }
        public Login Google { get; set; }
        public Login Facebook { get; set; }
    }

    public class Login
    {
        public bool Enable { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
