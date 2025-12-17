// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace Autolynx.Core.Services;

public class OpenAIClientWrapper : IOpenAIClientWrapper
{
    private readonly AzureOpenAIClient _client;

    public OpenAIClientWrapper(IConfiguration configuration)
    {
        var endpoint = configuration["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is not configured");
        var apiKey = configuration["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured");

        _client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    public async Task<string> GetChatCompletionAsync(string deploymentName, string prompt, CancellationToken cancellationToken = default)
    {
        var chatClient = _client.GetChatClient(deploymentName);

        var messages = new ChatMessage[]
        {
            new SystemChatMessage("You are a helpful assistant that searches for vehicle listings on automotive websites."),
            new UserChatMessage(prompt)
        };

        var completion = await chatClient.CompleteChatAsync(
            messages,
            cancellationToken: cancellationToken);

        return completion.Value.Content[0].Text;
    }
}
