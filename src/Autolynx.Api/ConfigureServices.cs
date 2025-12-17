// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Api.Hubs;
using Autolynx.Core.Options;
using Autolynx.Core.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(nameof(CorsOptions)).Get<CorsOptions>() ?? new CorsOptions();
        
        services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder => builder
            .WithOrigins(corsOptions.AllowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(isOriginAllowed: _ => true)
            .AllowCredentials()));

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Add Options
        services.Configure<BingSearchOptions>(configuration.GetSection(nameof(BingSearchOptions)));

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Autolynx.Core.Services.IVehicleSearchService).Assembly));

        // Add SignalR
        services.AddSignalR();

        // Add Core Services
        services.AddHttpClient<IBingSearchService, BingSearchService>();
        services.AddScoped<IVehicleSearchService, VehicleSearchService>();
    }
}