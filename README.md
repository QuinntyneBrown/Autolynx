# Autolynx

Autolynx is an intelligent vehicle search platform that helps users find vehicles for sale based on their specific criteria using Azure OpenAI to search across multiple automotive marketplaces.

## Features

- **AI-Powered Vehicle Search**: Uses Azure OpenAI to search for vehicles across popular platforms like Car Gurus, Clutch, AutoTrader, Kijiji, and more
- **Flexible Search Criteria**: Search by make, model, year range, price range, location (country, province, city, postal code), transmission type, and fuel type
- **Real-time Updates**: SignalR integration provides live search progress and results
- **Price Intelligence**: Get indicators on whether a vehicle listing is a good deal
- **Comprehensive Results**: Includes vehicle details, pricing, dealer information, and direct links to listings

## Architecture

- **Autolynx.Core**: Domain models, services, and MediatR CQRS handlers
- **Autolynx.Api**: ASP.NET Core Web API with SignalR support
- **Autolynx.Testing**: Fakes and test utilities for integration testing
- **Autolynx.Core.UnitTests**: Unit tests for core services
- **Autolynx.Api.IntegrationTests**: Integration tests for API endpoints

## Prerequisites

- .NET 9.0 SDK or later
- Azure OpenAI account with an active deployment

## Configuration

Update `appsettings.json` in the `Autolynx.Api` project with your Azure OpenAI credentials:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource-name.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "your-deployment-name"
  }
}
```

**Note**: For production, use Azure Key Vault or environment variables to store sensitive credentials.

## Building the Project

```bash
dotnet restore Autolynx.sln
dotnet build Autolynx.sln
```

## Running Tests

```bash
# Run all tests
dotnet test Autolynx.sln

# Run unit tests only
dotnet test test/Autolynx.Core.UnitTests/Autolynx.Core.UnitTests.csproj

# Run integration tests only
dotnet test test/Autolynx.Api.IntegrationTests/Autolynx.Api.IntegrationTests.csproj
```

## Running the API

```bash
cd src/Autolynx.Api
dotnet run
```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

## API Endpoints

### Search for Vehicles

**POST** `/api/vehicles/search`

Search for vehicles based on specific criteria.

#### Request Body

```json
{
  "make": "Toyota",
  "model": "Camry",
  "yearMin": 2020,
  "yearMax": 2024,
  "priceMin": 20000,
  "priceMax": 35000,
  "mileageMax": 50000,
  "country": "Canada",
  "province": "Ontario",
  "city": "Toronto",
  "postalCode": "M5H 2N2",
  "transmission": "Automatic",
  "fuelType": "Gasoline"
}
```

All fields are optional. Provide only the criteria that matter to your search.

#### Response

```json
[
  {
    "make": "Toyota",
    "model": "Camry",
    "year": 2023,
    "trim": "SE",
    "mileage": 15000,
    "color": "Silver",
    "transmission": "Automatic",
    "fuelType": "Gasoline",
    "price": 28500,
    "isGoodPrice": true,
    "listingUrl": "https://www.autotrader.ca/listing/12345",
    "dealerName": "ABC Motors",
    "sellerPhone": "555-1234",
    "sellerEmail": "sales@abcmotors.com",
    "location": "Toronto, ON",
    "source": "AutoTrader",
    "vin": "1HGBH41JXMN109186"
  }
]
```

## SignalR Integration

Connect to the SignalR hub to receive real-time search updates:

**Hub URL**: `/hubs/vehicle-search`

### Events

- `ReceiveSearchProgress`: Receives progress updates during the search
- `ReceiveVehicleResult`: Receives individual vehicle results as they are found
- `ReceiveSearchComplete`: Notifies when the search is complete with total result count

### JavaScript Client Example

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/hubs/vehicle-search")
    .build();

connection.on("ReceiveSearchProgress", (message) => {
    console.log("Progress:", message);
});

connection.on("ReceiveVehicleResult", (vehicle) => {
    console.log("Found vehicle:", vehicle);
});

connection.on("ReceiveSearchComplete", (count) => {
    console.log("Search complete. Found", count, "vehicles");
});

await connection.start();
```

## CI/CD

The project includes a GitHub Actions workflow that automatically:
- Builds the solution
- Runs all unit tests
- Runs all integration tests
- Reports test results

The workflow runs on every pull request to the main branch.

## Testing Strategy

- **Unit Tests**: Test individual services with mocked dependencies
- **Integration Tests**: Test API endpoints using `WebApplicationFactory` with fake Azure OpenAI client
- **Fakes**: The `Autolynx.Testing` project provides fake implementations for testing without requiring actual Azure OpenAI access

## Design Patterns

- **CQRS**: Using MediatR for command/query separation
- **Repository Pattern**: Service interfaces abstract data access
- **Dependency Injection**: All services are injected via ASP.NET Core DI
- **Fake Pattern**: Test doubles for external dependencies

## License

Copyright (c) Quinntyne Brown. All Rights Reserved.
Licensed under the MIT License. See License.txt in the project root for license information.
