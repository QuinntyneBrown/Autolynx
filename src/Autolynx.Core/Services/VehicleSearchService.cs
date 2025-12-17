// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;
using Autolynx.Core.Models;
using Microsoft.Extensions.Logging;

namespace Autolynx.Core.Services;

public class VehicleSearchService : IVehicleSearchService
{
    private readonly IBingSearchService _bingSearchService;
    private readonly ILogger<VehicleSearchService> _logger;

    public VehicleSearchService(
        IBingSearchService bingSearchService,
        ILogger<VehicleSearchService> logger)
    {
        _bingSearchService = bingSearchService ?? throw new ArgumentNullException(nameof(bingSearchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<VehicleSearchResultDto>> SearchVehiclesAsync(VehicleSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching for vehicles with criteria: {Criteria}", JsonSerializer.Serialize(criteria));

        var searchQuery = BuildBingSearchQuery(criteria);

        try
        {
            var response = await _bingSearchService.SearchAsync(searchQuery, cancellationToken);
            var results = ParseBingResults(response, criteria);

            _logger.LogInformation("Found {Count} vehicles matching criteria", results.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for vehicles");
            throw;
        }
    }

    private string BuildBingSearchQuery(VehicleSearchCriteria criteria)
    {
        var queryParts = new List<string>();

        if (!string.IsNullOrEmpty(criteria.Make))
            queryParts.Add(criteria.Make);

        if (!string.IsNullOrEmpty(criteria.Model))
            queryParts.Add(criteria.Model);

        if (criteria.YearMin.HasValue)
            queryParts.Add(criteria.YearMin.Value.ToString());

        // Add common automotive search terms
        queryParts.Add("for sale");

        var location = BuildLocationString(criteria);
        if (!string.IsNullOrEmpty(location))
            queryParts.Add(location);

        // Add site restrictions for popular automotive sites
        var siteRestrictions = new[] 
        { 
            "site:cars.com OR site:autotrader.com OR site:cargurus.com OR site:carfax.com OR site:edmunds.com"
        };
        queryParts.Add($"({string.Join(" OR ", siteRestrictions)})");

        return string.Join(" ", queryParts);
    }

    private string BuildLocationString(VehicleSearchCriteria criteria)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(criteria.City))
            parts.Add(criteria.City);

        if (!string.IsNullOrWhiteSpace(criteria.Province))
            parts.Add(criteria.Province);

        if (!string.IsNullOrWhiteSpace(criteria.Country))
            parts.Add(criteria.Country);

        return string.Join(", ", parts);
    }

    private List<VehicleSearchResultDto> ParseBingResults(string response, VehicleSearchCriteria criteria)
    {
        try
        {
            var bingResponse = JsonSerializer.Deserialize<BingSearchResponse>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (bingResponse?.WebPages?.Value == null || !bingResponse.WebPages.Value.Any())
            {
                _logger.LogWarning("No web pages found in Bing search response");
                return new List<VehicleSearchResultDto>();
            }

            var results = new List<VehicleSearchResultDto>();

            foreach (var page in bingResponse.WebPages.Value)
            {
                // Extract vehicle information from search results
                var vehicle = new VehicleSearchResultDto
                {
                    Make = criteria.Make,
                    Model = criteria.Model,
                    Year = ExtractYear(page.Name, page.Snippet, criteria),
                    ListingUrl = page.Url,
                    Source = ExtractSource(page.DisplayUrl),
                    Location = ExtractLocation(page.Snippet, criteria),
                    Price = ExtractPrice(page.Snippet),
                    Mileage = ExtractMileage(page.Snippet),
                    IsGoodPrice = false // TODO: Implement price analysis logic
                };

                results.Add(vehicle);
            }

            return results;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing Bing search results");
            return new List<VehicleSearchResultDto>();
        }
    }

    private int ExtractYear(string title, string snippet, VehicleSearchCriteria criteria)
    {
        // Try to extract year from title or snippet
        var text = $"{title} {snippet}";
        var yearMatch = System.Text.RegularExpressions.Regex.Match(text, @"\b(19|20)\d{2}\b");
        
        if (yearMatch.Success && int.TryParse(yearMatch.Value, out var year))
            return year;

        // Default to criteria year if available
        return criteria.YearMin ?? DateTime.Now.Year;
    }

    private string ExtractSource(string displayUrl)
    {
        var domain = displayUrl.Split('/')[0].Replace("www.", "");
        return domain;
    }

    private string? ExtractLocation(string snippet, VehicleSearchCriteria criteria)
    {
        // Return criteria location if available
        if (!string.IsNullOrEmpty(criteria.City))
            return $"{criteria.City}, {criteria.Province ?? criteria.Country}";

        return null;
    }

    private decimal ExtractPrice(string snippet)
    {
        // Try to extract price from snippet
        var priceMatch = System.Text.RegularExpressions.Regex.Match(snippet, @"\$([\d,]+)");
        
        if (priceMatch.Success)
        {
            var priceStr = priceMatch.Groups[1].Value.Replace(",", "");
            if (decimal.TryParse(priceStr, out var price))
                return price;
        }

        return 0;
    }

    private int ExtractMileage(string snippet)
    {
        // Try to extract mileage from snippet
        var mileageMatch = System.Text.RegularExpressions.Regex.Match(snippet, @"([\d,]+)\s*(miles|mi|km)");
        
        if (mileageMatch.Success)
        {
            var mileageStr = mileageMatch.Groups[1].Value.Replace(",", "");
            if (int.TryParse(mileageStr, out var mileage))
                return mileage;
        }

        return 0;
    }
}
