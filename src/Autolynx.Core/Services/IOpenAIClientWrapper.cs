// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Services;

public interface IOpenAIClientWrapper
{
    Task<string> GetChatCompletionAsync(string deploymentName, string prompt, CancellationToken cancellationToken = default);
    Task<string> GetChatCompletionWithGroundingAsync(string deploymentName, string prompt, CancellationToken cancellationToken = default);
}
