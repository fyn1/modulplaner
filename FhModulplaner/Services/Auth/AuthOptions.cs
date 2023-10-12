namespace FhModulplaner.Services.Auth;

public class AuthOptions
{
    public const string AuthenticationSectionName = "Auth";
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}