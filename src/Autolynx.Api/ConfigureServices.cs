// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Api.Hubs;
using Autolynx.Core.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(isOriginAllowed: _ => true)
            .AllowCredentials()));

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Autolynx.Core.Services.IVehicleSearchService).Assembly));

        // Add SignalR
        services.AddSignalR();

        // Add Core Services
        services.AddSingleton<IOpenAIClientWrapper, OpenAIClientWrapper>();
        services.AddScoped<IVehicleSearchService, VehicleSearchService>();
    }
}