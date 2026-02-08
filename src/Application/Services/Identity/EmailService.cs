using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Net.Http.Headers;
using System.Text;

namespace Application.Services.Identity;
public class EmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _mailgunApiKey;
    private readonly string _mailgunDomain;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, HttpClient httpClient, ILogger<EmailService> logger)
    {
        _mailgunApiKey = configuration["Mailgun:ApiKey"];
        _mailgunDomain = configuration["Mailgun:Domain"];
        if (string.IsNullOrEmpty(_mailgunApiKey) || string.IsNullOrEmpty(_mailgunDomain))
            throw new InvalidOperationException("Api Key and BaseUrl are required");
        _httpClient = httpClient;
        _logger = logger;

        // Set up basic authentication for Mailgun
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_mailgunApiKey}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("from", $"Skill Circle Support <support@{_mailgunDomain}>"),
            new KeyValuePair<string, string>("to", toEmail),
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("html", htmlContent)  // Send HTML email content
        });

        var response = await _httpClient.PostAsync($"https://api.mailgun.net/v3/{_mailgunDomain}/messages", requestContent);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error sending email: {response.StatusCode}");
            throw new InvalidOperationException($"Error sending email: {response.StatusCode}");
        }
    }
}
