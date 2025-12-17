// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;
using Autolynx.Core.Models;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace Autolynx.Core.Services;

public class VehicleSearchService : IVehicleSearchService
{
    private readonly IOpenAIClientWrapper _openAIClient;
    private readonly ILogger<VehicleSearchService> _logger;
    private readonly string _deploymentName;

    public VehicleSearchService(
        IOpenAIClientWrapper openAIClient,
        IConfiguration configuration,
        ILogger<VehicleSearchService> logger)
    {
        _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName is not configured");
    }

    public async Task<List<VehicleSearchResultDto>> SearchVehiclesAsync(VehicleSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching for vehicles with criteria: {Criteria}", JsonSerializer.Serialize(criteria));

        var prompt = BuildSearchPrompt(criteria);

        try
        {
            var response = await _openAIClient.GetChatCompletionAsync(_deploymentName, prompt, cancellationToken);
            var results = ParseVehicleResults(response);

            _logger.LogInformation("Found {Count} vehicles matching criteria", results.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for vehicles");
            throw;
        }
    }

    private string BuildSearchPrompt(VehicleSearchCriteria criteria)
    {
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("Search for vehicles for sale based on the following criteria:");
        promptBuilder.AppendLine("Please search popular automotive websites like Car Gurus, Clutch, AutoTrader, Kijiji, and other relevant platforms.");
        promptBuilder.AppendLine();

        if (!string.IsNullOrWhiteSpace(criteria.Make))
            promptBuilder.AppendLine($"Make: {criteria.Make}");

        if (!string.IsNullOrWhiteSpace(criteria.Model))
            promptBuilder.AppendLine($"Model: {criteria.Model}");

        if (criteria.YearMin.HasValue || criteria.YearMax.HasValue)
            promptBuilder.AppendLine($"Year Range: {criteria.YearMin ?? 1900} - {criteria.YearMax ?? DateTime.Now.Year}");

        if (criteria.PriceMin.HasValue || criteria.PriceMax.HasValue)
            promptBuilder.AppendLine($"Price Range: ${criteria.PriceMin ?? 0:N2} - ${criteria.PriceMax ?? 999999:N2}");

        if (criteria.MileageMax.HasValue)
            promptBuilder.AppendLine($"Max Mileage: {criteria.MileageMax} km");

        if (!string.IsNullOrWhiteSpace(criteria.Country))
            promptBuilder.AppendLine($"Country: {criteria.Country}");

        if (!string.IsNullOrWhiteSpace(criteria.Province))
            promptBuilder.AppendLine($"Province/State: {criteria.Province}");

        if (!string.IsNullOrWhiteSpace(criteria.City))
            promptBuilder.AppendLine($"City: {criteria.City}");

        if (!string.IsNullOrWhiteSpace(criteria.PostalCode))
            promptBuilder.AppendLine($"Postal Code: {criteria.PostalCode}");

        if (!string.IsNullOrWhiteSpace(criteria.Transmission))
            promptBuilder.AppendLine($"Transmission: {criteria.Transmission}");

        if (!string.IsNullOrWhiteSpace(criteria.FuelType))
            promptBuilder.AppendLine($"Fuel Type: {criteria.FuelType}");

        promptBuilder.AppendLine();
        promptBuilder.AppendLine("For each vehicle found, provide the following information in JSON format:");
        promptBuilder.AppendLine("- Make, Model, Year, Trim");
        promptBuilder.AppendLine("- Mileage, Color, Transmission, FuelType");
        promptBuilder.AppendLine("- Price");
        promptBuilder.AppendLine("- IsGoodPrice (boolean indicating if this is a good deal based on market value)");
        promptBuilder.AppendLine("- ListingUrl (the public URL to view the listing)");
        promptBuilder.AppendLine("- DealerName, SellerPhone, SellerEmail");
        promptBuilder.AppendLine("- Location (full location string)");
        promptBuilder.AppendLine("- Source (website name)");
        promptBuilder.AppendLine("- VIN (if available)");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Return results as a JSON array of vehicle objects.");

        return promptBuilder.ToString();
    }

    private List<VehicleSearchResultDto> ParseVehicleResults(string responseContent)
    {
        try
        {
            // Try to extract JSON array from the response
            var jsonStart = responseContent.IndexOf('[');
            var jsonEnd = responseContent.LastIndexOf(']');

            if (jsonStart == -1 || jsonEnd == -1)
            {
                _logger.LogWarning("No JSON array found in response");
                return new List<VehicleSearchResultDto>();
            }

            var jsonContent = responseContent.Substring(jsonStart, jsonEnd - jsonStart + 1);
            var results = JsonSerializer.Deserialize<List<VehicleSearchResultDto>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return results ?? new List<VehicleSearchResultDto>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing vehicle search results from response: {Response}", responseContent);
            return new List<VehicleSearchResultDto>();
        }
    }
}
