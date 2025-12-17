namespace Autolynx.Core.Options;

public class BingSearchOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.bing.microsoft.com/v7.0/search";
    public int ResultCount { get; set; } = 10;
    public string Market { get; set; } = "en-US";
}
