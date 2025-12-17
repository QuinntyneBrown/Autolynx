// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;
using Autolynx.Core.Options;
using Autolynx.Core.Services;
using Autolynx.Testing.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Autolynx.Core.UnitTests;

public class VehicleSearchServiceTests
{
    private readonly Mock<ILogger<VehicleSearchService>> _loggerMock;
    private readonly IOptions<AzureOpenAIOptions> _options;

    public VehicleSearchServiceTests()
    {
        _loggerMock = new Mock<ILogger<VehicleSearchService>>();
        _options = Microsoft.Extensions.Options.Options.Create(new AzureOpenAIOptions
        {
            Endpoint = "https://test.openai.azure.com/",
            ApiKey = "test-key",
            DeploymentName = "test-deployment"
        });
    }

    [Fact]
    public async Task SearchVehiclesAsync_ShouldReturnResults_WhenCriteriaProvided()
    {
        // Arrange
        var fakeClient = new FakeOpenAIClientWrapper();
        var service = new VehicleSearchService(fakeClient, _options, _loggerMock.Object);
        var criteria = new VehicleSearchCriteria
        {
            Make = "Toyota",
            Model = "Camry",
            YearMin = 2020,
            YearMax = 2024,
            City = "Toronto"
        };

        // Act
        var results = await service.SearchVehiclesAsync(criteria);

        // Assert
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Make == "Toyota");
        Assert.Contains(results, r => r.Make == "Honda");
    }

    [Fact]
    public async Task SearchVehiclesAsync_ShouldReturnEmptyList_WhenNoResultsFound()
    {
        // Arrange
        var fakeClient = new FakeOpenAIClientWrapper(new List<VehicleSearchResultDto>());
        var service = new VehicleSearchService(fakeClient, _options, _loggerMock.Object);
        var criteria = new VehicleSearchCriteria
        {
            Make = "RareBrand",
            Model = "RareModel"
        };

        // Act
        var results = await service.SearchVehiclesAsync(criteria);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchVehiclesAsync_ShouldIncludeLocationInResults()
    {
        // Arrange
        var fakeClient = new FakeOpenAIClientWrapper();
        var service = new VehicleSearchService(fakeClient, _options, _loggerMock.Object);
        var criteria = new VehicleSearchCriteria
        {
            Province = "ON"
        };

        // Act
        var results = await service.SearchVehiclesAsync(criteria);

        // Assert
        Assert.NotNull(results);
        Assert.All(results, r => Assert.NotNull(r.Location));
    }

    [Fact]
    public void VehicleSearchService_ShouldThrowException_WhenDeploymentNameNotConfigured()
    {
        // Arrange
        var fakeClient = new FakeOpenAIClientWrapper();
        var optionsWithoutDeployment = Microsoft.Extensions.Options.Options.Create(new AzureOpenAIOptions
        {
            Endpoint = "https://test.openai.azure.com/",
            ApiKey = "test-key",
            DeploymentName = "" // Empty deployment name
        });

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new VehicleSearchService(fakeClient, optionsWithoutDeployment, _loggerMock.Object));
        
        Assert.Contains("DeploymentName", exception.Message);
    }
}
