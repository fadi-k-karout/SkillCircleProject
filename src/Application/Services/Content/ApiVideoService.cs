using Application.Common.Interfaces.Content;
using Domain.Models;
namespace Application.Services.Content;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class ApiVideoService : ICdnService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl; 
    

    public ApiVideoService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ApiVideo:ApiKey"];
        _baseUrl = configuration["ApiVideo:BaseUrl"]; 
        if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_baseUrl))
            throw new InvalidOperationException("Api Key and BaseUrl are required");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }


    public async Task<string> GenerateUploadTokenAsync()
    {
      
        var expiresInHours = 12;
        
        // Set the expiration timestamp in seconds
        var expiresIn = DateTimeOffset.UtcNow.AddHours(expiresInHours).ToUnixTimeSeconds();

        var payload = new
        {
            ttl = expiresIn
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

        // Endpoint for creating delegated upload token
        var response = await _httpClient.PostAsync($"{_baseUrl}/auth/delegated-upload-tokens", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to generate upload token. Status Code: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DelegatedUploadResponse>(jsonResponse);

        return result?.Token ?? throw new Exception("Invalid response from api.video");
    }
    
    public async Task<Dictionary<string, bool>> CheckUploadStatusAsync(List<string> videoIds)
    {
        var results = new Dictionary<string, bool>();
      
        foreach (var videoId in videoIds)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/videos/{videoId}/status");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync(); // Get the JSON response



                // Optionally deserialize the response if you need to check properties
                var videoStatusResponse = JsonSerializer.Deserialize<VideoStatusResponse>(jsonResponse);

                bool isUploaded = videoStatusResponse.Ingest.Status == "uploaded";
                

                results[videoId] = isUploaded;
            }
            else
            {
                results[videoId] = false; 
            }
        }

        return results;
    }

    private class DelegatedUploadResponse
         {
             public string Token { get; set; }
         }

    private class VideoStatusResponse
    {
        public Ingest Ingest { get; set; }
        public EncodingStatus Encoding { get; set; }
    }

    private class Ingest
    {
        public string Status { get; set; }
    }

    private class EncodingStatus
    {
        public bool Playable { get; set; }
    }
}
