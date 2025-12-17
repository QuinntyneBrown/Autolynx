// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using Autolynx.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autolynx.Core.Services;

public class BingSearchService : IBingSearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BingSearchService> _logger;
    private readonly BingSearchOptions _options;

    public BingSearchService(
        HttpClient httpClient,
        IOptions<BingSearchOptions> options,
        ILogger<BingSearchService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value;

        if (string.IsNullOrEmpty(_options.ApiKey))
            throw new InvalidOperationException("BingSearchOptions:ApiKey is not configured");

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_options.Endpoint);
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _options.ApiKey);
    }

    public async Task<string> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing Bing search with query: {Query}", query);

        try
        {
            var requestUri = $"?q={Uri.EscapeDataString(query)}&count={_options.ResultCount}&mkt={_options.Market}";
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("Bing search completed successfully");

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Bing search");
            throw;
        }
    }
}
