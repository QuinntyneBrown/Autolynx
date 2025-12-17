// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Api.Hubs;
using Autolynx.Core.Features.VehicleSearch;
using Autolynx.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Autolynx.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<VehicleSearchHub> _hubContext;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(
        IMediator mediator,
        IHubContext<VehicleSearchHub> hubContext,
        ILogger<VehiclesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Search for vehicles based on criteria
    /// </summary>
    /// <param name="criteria">The search criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of vehicle search results</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(List<VehicleSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<VehicleSearchResultDto>>> SearchVehicles(
        [FromBody] VehicleSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received vehicle search request");

            // Send progress update via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSearchProgress", "Search started...", cancellationToken);

            var query = new SearchVehiclesQuery { Criteria = criteria };
            var results = await _mediator.Send(query, cancellationToken);

            // Send each result via SignalR
            foreach (var result in results)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveVehicleResult", result, cancellationToken);
            }

            // Send completion notification via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSearchComplete", results.Count, cancellationToken);

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for vehicles");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching for vehicles");
        }
    }
}
