using System.Text;
using Autolynx.Core.Features.VehicleSearch;
using Autolynx.Core.Models;
using Autolynx.Core.Options;
using Autolynx.Core.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.OutputEncoding = Encoding.UTF8;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

// Setup DI
var services = new ServiceCollection();

services.AddLogging();

// Add Options
services.Configure<AzureOpenAIOptions>(configuration.GetSection(nameof(AzureOpenAIOptions)));

// Add Configuration
services.AddSingleton<IConfiguration>(configuration);

// Add MediatR
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SearchVehiclesQuery).Assembly));

// Add Core Services
services.AddSingleton<IOpenAIClientWrapper, OpenAIClientWrapper>();
services.AddScoped<IVehicleSearchService, VehicleSearchService>();

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Autolynx CLI - Vehicle Search");
Console.WriteLine("===============================\n");

// Create search criteria
var criteria = new VehicleSearchCriteria
{
    Make = "Toyota",
    Model = "Camry",
    PostalCode = "L5A 4E6",
};

Console.WriteLine($"Searching for: {criteria.Make} {criteria.Model}");


try
{
    var mediator = serviceProvider.GetRequiredService<IMediator>();
    
    var query = new SearchVehiclesQuery
    {
        Criteria = criteria
    };
    
    var results = await mediator.Send(query);
    
    if (results != null && results.Any())
    {
        Console.WriteLine($"Found {results.Count} vehicle(s):\n");
        
        foreach (var result in results)
        {
            DisplayResult(result);
            Console.WriteLine();
        }
    }
    else
    {
        Console.WriteLine("No results found.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

static void DisplayResult(VehicleSearchResultDto result)
{
    Console.WriteLine(new string('=', 100));
    
    // Create table
    var rows = new List<(string Property, string Value)>
    {
        ("Make", result.Make ?? "N/A"),
        ("Model", result.Model ?? "N/A"),
        ("Year", result.Year.ToString()),
        ("Trim", result.Trim ?? "N/A"),
        ("Mileage", $"{result.Mileage:N0} miles"),
        ("Color", result.Color ?? "N/A"),
        ("Transmission", result.Transmission ?? "N/A"),
        ("Fuel Type", result.FuelType ?? "N/A"),
        ("Price", $"${result.Price:N2}"),
        ("Good Price?", result.IsGoodPrice ? "Yes ✓" : "No"),
        ("VIN", result.VIN ?? "N/A"),
        ("Dealer", result.DealerName ?? "N/A"),
        ("Location", result.Location ?? "N/A"),
        ("Phone", result.SellerPhone ?? "N/A"),
        ("Email", result.SellerEmail ?? "N/A"),
        ("Source", result.Source ?? "N/A"),
        ("Listing URL", result.ListingUrl ?? "N/A")
    };

    int maxPropertyLength = rows.Max(r => r.Property.Length);
    
    foreach (var (property, value) in rows)
    {
        Console.WriteLine($"  {property.PadRight(maxPropertyLength)} : {value}");
    }
    
    Console.WriteLine(new string('=', 100));
}
