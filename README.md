# Autolynx

Autolynx is an intelligent vehicle search platform that helps users find vehicles for sale across multiple automotive marketplaces using the Bing Search API.

## Features

- **Web Search Integration**: Uses Microsoft Bing Search API to find vehicles across popular platforms like Cars.com, AutoTrader, CarGurus, Carfax, Edmunds, and more
- **Flexible Search Criteria**: Search by make, model, year range, price range, location (country, province, city, postal code), transmission type, and fuel type
- **Real-time Updates**: SignalR integration provides live search progress and results
- **Intelligent Data Extraction**: Automatically extracts vehicle details (price, mileage, year) from search results
- **Comprehensive Results**: Includes vehicle details, pricing, dealer information, and direct links to live listings

## Architecture

### Projects

- **Autolynx.Core**: Domain models, services, MediatR CQRS handlers, and business logic
  - Features: Vehicle search using MediatR queries
  - Services: Bing Search integration, vehicle data extraction
  - Models: DTOs for vehicle search criteria and results
  - Options: Strongly-typed configuration classes (BingSearchOptions, CorsOptions)
  
- **Autolynx.Api**: ASP.NET Core Web API (.NET 9.0)
  - RESTful endpoints for vehicle search
  - SignalR hub for real-time updates
  - CORS configuration for frontend integration
  - Runs on: https://localhost:7182
  
- **Autolynx.WebApp**: Angular 19 frontend application
  - Vehicle search interface
  - Real-time results via SignalR
  - Environment-based configuration
  - Runs on: http://localhost:4200
  
- **Autolynx.Cli**: Command-line interface for vehicle search
  - Standalone console application
  - Uses MediatR and DI (same as API)
  - Displays results in formatted tables
  
- **Autolynx.Testing**: Fakes and test utilities for integration testing
- **Autolynx.Core.UnitTests**: Unit tests for core services
- **Autolynx.Api.IntegrationTests**: Integration tests for API endpoints

### Design Patterns

- **CQRS**: MediatR queries and handlers for separation of concerns
- **Options Pattern**: Strongly-typed configuration using IOptions<T>
- **Dependency Injection**: All services registered via ASP.NET Core DI
- **Repository Pattern**: Service interfaces abstract external dependencies
- **HttpClient Factory**: Managed HTTP clients for Bing Search API

## Prerequisites

- .NET 9.0 SDK or later
- Node.js 18+ and npm (for Angular frontend)
- Microsoft Bing Search API key (Free tier available: 1,000 searches/month)

## Getting Your Bing Search API Key

1. Go to [Azure Portal](https://portal.azure.com)
2. Create a "Bing Search v7" resource
3. Copy the API key from "Keys and Endpoint"
4. See [detailed integration guide](docs/BING-SEARCH-INTEGRATION.md) for step-by-step instructions

## Configuration

### API Configuration

Update `src/Autolynx.Api/appsettings.json`:

```json
{
  "BingSearchOptions": {
    "ApiKey": "your-bing-search-api-key-here",
    "Endpoint": "https://api.bing.microsoft.com/v7.0/search",
    "ResultCount": 10,
    "Market": "en-US"
  },
  "CorsOptions": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  }
}
```

### CLI Configuration

Update `src/Autolynx.Cli/appsettings.json` with the same `BingSearchOptions`.

### Frontend Configuration

Update `src/Autolynx.WebApp/src/environments/environment.development.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7182/api',
  hubUrl: 'https://localhost:7182/hubs/vehicle-search'
};
```

**Note**: For production, use Azure Key Vault or User Secrets to store sensitive credentials.

## Building the Project

### Backend (.NET)

```bash
dotnet restore Autolynx.sln
dotnet build Autolynx.sln
```

### Frontend (Angular)

```bash
cd src/Autolynx.WebApp
npm install
```

## Running Tests

```bash
# Run all tests
dotnet test Autolynx.sln

# Run unit tests only
dotnet test test/Autolynx.Core.UnitTests/Autolynx.Core.UnitTests.csproj

# Run integration tests only
dotnet test test/Autolynx.Api.IntegrationTests/Autolynx.Api.IntegrationTests.csproj

# Run Angular e2e tests
cd src/Autolynx.WebApp
npx playwright test
```

##GET** `/api/vehicles/search`

Search for vehicles based on specific criteria using query parameters.

#### Query Parameters

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| make | string | Vehicle manufacturer | Toyota |
| model | string | Vehicle model | Camry |
| yearMin | integer | Minimum year | 2020 |
| yearMax | integer | Maximum year | 2024 |
| priceMin | decimal | Minimum price | 20000 |
| priceMax | decimal | Maximum price | 35000 |
| mileageMax | integer | Maximum mileage | 50000 |
| country | string | Country | Canada |
| province | string | Province/State | Ontario |
| city | string | City | Toronto |
| postalCode | string | Postal/ZIP code | M5H 2N2 |
| transmission | string | Transmission type | Automatic |
| fuelType | string | Fuel type | Gasoline |

All parameters are optional. Provide only the criteria that matter to your search.

#### Example Request

```bash
GET /api/vehicles/search?make=Toyota&model=Camry&yearMin=2020&priceMax=30000
```
The CLI will execute a sample vehicle search and display results in a formatted table.

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

## SignalR Inthttps://localhost:7182/hubs/vehicle-search`

### Events

- `ReceiveSearchProgress`: Receives progress updates during the search
- `ReceiveVehicleResult`: Receives individual vehicle results as they are found
- `ReceiveSearchComplete`: Notifies when the search is complete with total result count

### JavaScript/TypeScript Client Example

```javascript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7182/hubs/vehicle-search")
    .withAutomaticReconnect(

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

conTechnology Stack

### Backend
- **.NET 9.0**: Latest version of .NET
- **ASP.NET Core**: Web API framework
- **MediatR**: CQRS pattern implementation
- **SignalR**: Real-time web functionality
- **Bing Search API**: Microsoft Cognitive Services for web search

### Frontend
- **Angular 19**: Latest Angular framework
- **TypeScript**: Type-safe JavaScript
- **SCSS**: Styling
- **SignalR Client**: Real-time updates
- **Playwright**: End-to-end testing

### Testing
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework (if needed)
- **WebApplicationFactory**: Integration testing
- **Playwright**: E2E testing for Angular

## CI/CD

The project includes a GitHub Actions workflow that automatically:
- Builds the solution
- Runs all unit tests
- Runs all integration tests
- Reports test results

The workflow runs on every pull request to the main branch.

## Testing Strategy

- **Unit Tests**: Test individual services with mocked dependencies
- **Integration Tests**: Test API endpoints using `WebApplicationFactory`
- **E2E Tests**: Playwright tests for Angular frontend
- **Fakes**: The `Autolynx.Testing` project provides fake implementations for testing

## How It Works

1. User submits search criteria through the Angular frontend or CLI
2. Request is sent to the API which creates a `SearchVehiclesQuery`
3. MediatR dispatches the query to `SearchVehiclesQueryHandler`
4. Handler calls `VehicleSearchService` which builds a Bing search query
5. `BingSearchService` makes HTTP request to Bing Search API
6. Results are parsed and vehicle data is extracted using regex patterns
7. Results are returned to the client (or displayed in CLI table)
8. SignalR hub broadcasts real-time updates to connected clients

## Additional Documentation

- [Bing Search API Integration Guide](docs/BING-SEARCH-INTEGRATION.md) - Comprehensive guide with tutorials and example implementations for testing without requiring actual Azure OpenAI access

## Design Patterns

- **CQRS**: Using MediatR for command/query separation
- **Repository Pattern**: Service interfaces abstract data access
- **Dependency Injection**: All services are injected via ASP.NET Core DI
- **Fake Pattern**: Test doubles for external dependencies

## License

Copyright (c) Quinntyne Brown. All Rights Reserved.
Licensed under the MIT License. See License.txt in the project root for license information.
