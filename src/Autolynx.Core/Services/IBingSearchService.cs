// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Services;

public interface IBingSearchService
{
    Task<string> SearchAsync(string query, CancellationToken cancellationToken = default);
}
