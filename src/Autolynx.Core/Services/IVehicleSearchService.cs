// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;

namespace Autolynx.Core.Services;

public interface IVehicleSearchService
{
    Task<List<VehicleSearchResultDto>> SearchVehiclesAsync(VehicleSearchCriteria criteria, CancellationToken cancellationToken = default);
}
