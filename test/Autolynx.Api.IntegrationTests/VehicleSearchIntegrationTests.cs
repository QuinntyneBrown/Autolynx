// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http.Json;
using Autolynx.Core.Models;
using Autolynx.Core.Services;
using Autolynx.Testing.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Autolynx.Api.IntegrationTests;

public class VehicleSearchIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public VehicleSearchIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the real OpenAI client with the fake
                services.RemoveAll<IOpenAIClientWrapper>();
                services.AddSingleton<IOpenAIClientWrapper, FakeOpenAIClientWrapper>();
            });
        });
    }

    [Fact]
    public async Task SearchVehicles_ShouldReturnOk_WithValidCriteria()
    {
        // Arrange
        var client = _factory.CreateClient();
        var criteria = new VehicleSearchCriteria
        {
            Make = "Toyota",
            Model = "Camry",
            YearMin = 2020,
            YearMax = 2024,
            City = "Toronto",
            Province = "ON"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles/search", criteria);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<VehicleSearchResultDto>>();
        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task SearchVehicles_ShouldReturnResults_WithMinimalCriteria()
    {
        // Arrange
        var client = _factory.CreateClient();
        var criteria = new VehicleSearchCriteria
        {
            Make = "Honda"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles/search", criteria);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<VehicleSearchResultDto>>();
        Assert.NotNull(results);
    }

    [Fact]
    public async Task SearchVehicles_ShouldIncludeRequiredFields_InResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        var criteria = new VehicleSearchCriteria
        {
            Make = "Toyota",
            PostalCode = "M5H 2N2"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles/search", criteria);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<VehicleSearchResultDto>>();
        Assert.NotNull(results);

        foreach (var result in results)
        {
            Assert.NotNull(result.Make);
            Assert.NotNull(result.Model);
            Assert.True(result.Year > 0);
            Assert.NotNull(result.ListingUrl);
            Assert.NotNull(result.Source);
        }
    }
}
