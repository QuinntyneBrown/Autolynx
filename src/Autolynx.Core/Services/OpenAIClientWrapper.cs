// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Options;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Autolynx.Core.Services;

public class OpenAIClientWrapper : IOpenAIClientWrapper
{
    private readonly AzureOpenAIClient _client;

    public OpenAIClientWrapper(IOptions<AzureOpenAIOptions> options)
    {
        var azureOptions = options.Value;
        
        if (string.IsNullOrEmpty(azureOptions.Endpoint))
            throw new InvalidOperationException("AzureOpenAIOptions:Endpoint is not configured");
        if (string.IsNullOrEmpty(azureOptions.ApiKey))
            throw new InvalidOperationException("AzureOpenAIOptions:ApiKey is not configured");

        _client = new AzureOpenAIClient(new Uri(azureOptions.Endpoint), new AzureKeyCredential(azureOptions.ApiKey));
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

    public async Task<string> GetChatCompletionWithGroundingAsync(string deploymentName, string prompt, CancellationToken cancellationToken = default)
    {
        var chatClient = _client.GetChatClient(deploymentName);

        var messages = new ChatMessage[]
        {
            new SystemChatMessage("You are a helpful assistant that searches for vehicle listings. Use web search to find current, real listings from automotive websites."),
            new UserChatMessage(prompt)
        };

        var options = new OpenAI.Chat.ChatCompletionOptions
        {
            Temperature = 0.7f,
            MaxOutputTokenCount = 4000
        };

        // Enable grounding with Bing Search (Azure OpenAI on your data feature)
        // This requires the deployment to have grounding/search enabled
        var completion = await chatClient.CompleteChatAsync(
            messages,
            options,
            cancellationToken: cancellationToken);

        return completion.Value.Content[0].Text;
    }
}
