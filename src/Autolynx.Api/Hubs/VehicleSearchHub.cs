// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace Autolynx.Api.Hubs;

public class VehicleSearchHub : Hub
{
    public async Task SendSearchProgress(string message)
    {
        await Clients.All.SendAsync("ReceiveSearchProgress", message);
    }

    public async Task SendVehicleResult(VehicleSearchResultDto result)
    {
        await Clients.All.SendAsync("ReceiveVehicleResult", result);
    }

    public async Task SendSearchComplete(int totalResults)
    {
        await Clients.All.SendAsync("ReceiveSearchComplete", totalResults);
    }
}
