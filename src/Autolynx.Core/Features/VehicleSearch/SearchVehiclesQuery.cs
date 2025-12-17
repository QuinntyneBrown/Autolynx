// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;
using MediatR;

namespace Autolynx.Core.Features.VehicleSearch;

public class SearchVehiclesQuery : IRequest<List<VehicleSearchResultDto>>
{
    public VehicleSearchCriteria Criteria { get; set; } = new();
}
