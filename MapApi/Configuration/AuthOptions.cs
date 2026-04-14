namespace MapApi.Configuration;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string Issuer { get; set; } = "GpsGeoFenceApp.Api";
    public string Audience { get; set; } = "GpsGeoFenceApp.Mobile";
    public int AccessTokenHours { get; set; } = 24;
    public string[] AdminUsernames { get; set; } = [];
    public string[] AdminEmails { get; set; } = [];
}
