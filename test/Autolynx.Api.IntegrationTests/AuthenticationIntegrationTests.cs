// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Autolynx.Api.Controllers;
using Autolynx.Core.Models;
using Autolynx.Core.Services;
using Autolynx.Testing.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Autolynx.Api.IntegrationTests;

public class AuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
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
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "testpassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.Equal("testuser", loginResponse.Username);
        Assert.NotEmpty(loginResponse.Roles);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest
        {
            Username = "",
            Password = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VehicleSearch_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        // Arrange
        var client = _factory.CreateClient();
        var criteria = new VehicleSearchCriteria
        {
            Make = "Toyota"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles/search", criteria);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VehicleSearch_ShouldReturnOk_WhenValidTokenProvided()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // First, get a valid token
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "testpassword"
        };
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        Assert.NotNull(loginResult);
        
        // Add token to request
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);
        
        var criteria = new VehicleSearchCriteria
        {
            Make = "Toyota",
            Model = "Camry"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles/search", criteria);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<VehicleSearchResultDto>>();
        Assert.NotNull(results);
    }
}
