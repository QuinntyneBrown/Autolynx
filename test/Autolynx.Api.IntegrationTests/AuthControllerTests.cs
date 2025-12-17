// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http.Json;
using Autolynx.Api.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Autolynx.Api.IntegrationTests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
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
    public async Task Login_ShouldReturnAdminRole_WhenUsernameIsAdmin()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "password"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.Contains("Admin", loginResponse.Roles);
    }

    [Fact]
    public async Task Login_ShouldReturnUserRole_WhenUsernameIsNotAdmin()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest
        {
            Username = "regularuser",
            Password = "password"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.Contains("User", loginResponse.Roles);
        Assert.DoesNotContain("Admin", loginResponse.Roles);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUsernameIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest
        {
            Username = "",
            Password = "password"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
