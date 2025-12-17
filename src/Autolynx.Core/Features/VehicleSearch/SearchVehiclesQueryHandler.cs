// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;
using Autolynx.Core.Services;
using MediatR;

namespace Autolynx.Core.Features.VehicleSearch;

public class SearchVehiclesQueryHandler : IRequestHandler<SearchVehiclesQuery, List<VehicleSearchResultDto>>
{
    private readonly IVehicleSearchService _vehicleSearchService;

    public SearchVehiclesQueryHandler(IVehicleSearchService vehicleSearchService)
    {
        _vehicleSearchService = vehicleSearchService ?? throw new ArgumentNullException(nameof(vehicleSearchService));
    }

    public async Task<List<VehicleSearchResultDto>> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        return await _vehicleSearchService.SearchVehiclesAsync(request.Criteria, cancellationToken);
    }
}
