// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using Autolynx.Core.Models;
using Autolynx.Core.Services;

namespace Autolynx.Testing.Fakes;

public class FakeOpenAIClientWrapper : IOpenAIClientWrapper
{
    private readonly List<VehicleSearchResultDto> _fakeResults;

    public FakeOpenAIClientWrapper()
    {
        _fakeResults = new List<VehicleSearchResultDto>
        {
            new VehicleSearchResultDto
            {
                Make = "Toyota",
                Model = "Camry",
                Year = 2023,
                Trim = "SE",
                Mileage = 15000,
                Color = "Silver",
                Transmission = "Automatic",
                FuelType = "Gasoline",
                Price = 28500,
                IsGoodPrice = true,
                ListingUrl = "https://example.com/listing1",
                DealerName = "ABC Motors",
                SellerPhone = "555-1234",
                SellerEmail = "sales@abcmotors.com",
                Location = "Toronto, ON",
                Source = "AutoTrader",
                VIN = "1HGBH41JXMN109186"
            },
            new VehicleSearchResultDto
            {
                Make = "Honda",
                Model = "Civic",
                Year = 2022,
                Trim = "Sport",
                Mileage = 25000,
                Color = "Blue",
                Transmission = "Manual",
                FuelType = "Gasoline",
                Price = 24000,
                IsGoodPrice = false,
                ListingUrl = "https://example.com/listing2",
                DealerName = "XYZ Auto",
                SellerPhone = "555-5678",
                SellerEmail = "info@xyzauto.com",
                Location = "Vancouver, BC",
                Source = "Car Gurus",
                VIN = "2HGFC2F59MH123456"
            }
        };
    }

    public FakeOpenAIClientWrapper(List<VehicleSearchResultDto> customResults)
    {
        _fakeResults = customResults;
    }

    public Task<string> GetChatCompletionAsync(string deploymentName, string prompt, CancellationToken cancellationToken = default)
    {
        var jsonResponse = JsonSerializer.Serialize(_fakeResults);
        return Task.FromResult(jsonResponse);
    }
}
