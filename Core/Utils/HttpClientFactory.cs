namespace Core.Utils;

public static class HttpClientFactory
{
    public static HttpClient Create(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));

        var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        return client;
    }
}
