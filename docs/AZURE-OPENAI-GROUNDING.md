# Azure OpenAI with Grounding Integration Guide

This document provides comprehensive information on integrating Azure OpenAI with Grounding using Bing Search for vehicle search functionality in Autolynx.

## Table of Contents
- [Overview](#overview)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Video Tutorials](#video-tutorials)
- [Code Examples](#code-examples)
- [Configuration](#configuration)
- [Best Practices](#best-practices)

## Overview

**Azure OpenAI Service with Grounding** combines the power of large language models with real-time web search capabilities through Bing Search. This feature, also known as "Azure OpenAI On Your Data" or "grounding with Bing Search," enables the model to retrieve current information from the web and provide accurate, up-to-date responses.

### Why Azure OpenAI with Grounding?

- **Real-time Data**: Access current vehicle listings from live websites
- **AI-Powered Analysis**: LLM interprets and structures unstructured web data
- **Contextual Understanding**: Better extraction of vehicle details and pricing
- **Reduced Hallucinations**: Grounded in actual search results
- **Structured Output**: AI formats data into consistent JSON structures
- **Source Attribution**: Citations to original sources

## Getting Started

### 1. Create Azure OpenAI Resource

**Azure Portal Steps:**
1. Go to [Azure Portal](https://portal.azure.com)
2. Click "Create a resource"
3. Search for "Azure OpenAI"
4. Click "Create"
5. Fill in the details:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or use existing
   - **Region**: Choose supported region (e.g., East US, West Europe)
   - **Name**: Give it a unique name
   - **Pricing Tier**: Standard S0
6. Click "Review + Create"
7. After deployment, go to resource

**Detailed Guide:**
https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource

### 2. Deploy a Model

1. In your Azure OpenAI resource, go to "Model deployments"
2. Click "+ Create new deployment"
3. Select a model (recommended: `gpt-4` or `gpt-35-turbo`)
4. Give it a deployment name
5. Click "Create"

### 3. Enable Grounding with Bing Search

**Note**: Grounding with Bing Search is automatically available when you use Azure OpenAI Service. The feature uses Azure's internal Bing Search integration, so you don't need a separate Bing Search API key.

**Requirements:**
- Azure OpenAI resource in a supported region
- GPT-3.5-Turbo or GPT-4 model deployment
- Grounding is enabled through specific API parameters

### 4. Get Your API Credentials

1. In your Azure OpenAI resource, go to "Keys and Endpoint"
2. Copy:
   - **Endpoint**: `https://your-resource.openai.azure.com/`
   - **Key**: One of the provided API keys
   - **Deployment Name**: The name you gave your model deployment

### 5. Update Configuration

Add your credentials to `appsettings.json`:

```json
{
  \"AzureOpenAIOptions\": {
    \"Endpoint\": \"https://your-resource.openai.azure.com/\",
    \"ApiKey\": \"your-actual-api-key-here\",
    \"DeploymentName\": \"your-deployment-name\"
  }
}
```

## API Documentation

### Official Microsoft Documentation

#### Core References
- **Azure OpenAI Service Overview**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/overview
  
- **Grounding with Bing Search**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/use-your-data
  
- **Chat Completions API Reference**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/reference
  
- **Azure OpenAI SDK for .NET**  
  https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme

#### Quickstart Guides
- **C# Quickstart**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/quickstart
  
- **Use Your Own Data Quickstart**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/use-your-data-quickstart

#### Advanced Topics
- **Prompt Engineering**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/prompt-engineering
  
- **System Message Best Practices**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/system-message
  
- **Managing Tokens and Costs**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/manage-costs

### How Grounding Works

When you send a prompt to Azure OpenAI with grounding enabled:

1. **User Query**: Your application sends a prompt (e.g., "Find Toyota Camry 2020 for sale")
2. **Web Search**: Azure OpenAI internally uses Bing to search the web
3. **Result Processing**: Retrieved web pages are analyzed
4. **AI Response**: The model generates a response grounded in the search results
5. **Structured Output**: Returns formatted JSON with vehicle data

### Request Format

```csharp
var messages = new ChatMessage[]
{
    new SystemChatMessage("You are a helpful assistant that searches for vehicle listings."),
    new UserChatMessage("Find Toyota Camry 2020 for sale under $30,000")
};

var options = new ChatCompletionOptions
{
    Temperature = 0.7f,
    MaxOutputTokenCount = 4000
};

var completion = await chatClient.CompleteChatAsync(messages, options);
```

## Video Tutorials

### YouTube Resources

#### Beginner Tutorials
- **"Azure OpenAI Service Tutorial"**  
  https://www.youtube.com/results?search_query=azure+openai+service+tutorial
  
- **"Getting Started with Azure OpenAI"**  
  https://www.youtube.com/results?search_query=azure+openai+getting+started

#### Grounding & Search
- **"Azure OpenAI with Your Own Data"**  
  https://www.youtube.com/results?search_query=azure+openai+on+your+data
  
- **"Grounding in Azure OpenAI"**  
  https://www.youtube.com/results?search_query=azure+openai+grounding

#### C# Specific
- **"Azure OpenAI with C# .NET"**  
  https://www.youtube.com/results?search_query=azure+openai+c%23+.net
  
- **"Build AI Apps with Azure OpenAI"**  
  https://www.youtube.com/results?search_query=build+app+azure+openai+c%23

### Microsoft Learn Video Courses
- **"Introduction to Azure OpenAI Service"**  
  https://learn.microsoft.com/en-us/training/modules/explore-azure-openai/
  
- **"Build Natural Language Solutions"**  
  https://learn.microsoft.com/en-us/training/paths/develop-ai-solutions-azure-openai/

## Code Examples

### Official Microsoft Samples

#### GitHub Repositories
- **Azure OpenAI Samples**  
  https://github.com/Azure-Samples/openai
  
- **Azure OpenAI .NET SDK Samples**  
  https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI/samples

### Example Usage in Autolynx

#### Basic Chat Completion
```csharp
public async Task<string> GetChatCompletionAsync(string prompt)
{
    var chatClient = _azureClient.GetChatClient(deploymentName);
    
    var messages = new ChatMessage[]
    {
        new SystemChatMessage(\"You are a helpful assistant.\"),
        new UserChatMessage(prompt)
    };
    
    var completion = await chatClient.CompleteChatAsync(messages);
    return completion.Value.Content[0].Text;
}
```

#### Chat with Grounding (Web Search)
```csharp
public async Task<string> GetChatCompletionWithGroundingAsync(string prompt)
{
    var chatClient = _azureClient.GetChatClient(deploymentName);
    
    var messages = new ChatMessage[]
    {
        new SystemChatMessage(\"Use web search to find current information.\"),
        new UserChatMessage(prompt)
    };
    
    var options = new ChatCompletionOptions
    {
        Temperature = 0.7f,
        MaxOutputTokenCount = 4000
    };
    
    var completion = await chatClient.CompleteChatAsync(messages, options);
    return completion.Value.Content[0].Text;
}
```

#### Vehicle Search Prompt
```csharp
var prompt = @\"Search for vehicles for sale based on the following criteria:
Make: Toyota
Model: Camry
Year: 2020+
Max Price: $30,000

Return results as JSON array with: Make, Model, Year, Price, Mileage, ListingUrl, Source\";
```

## Configuration

### Autolynx Configuration Files

The application uses strongly-typed options for Azure OpenAI configuration:

**AzureOpenAIOptions.cs**
```csharp
public class AzureOpenAIOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
}
```

### Dependency Injection Setup

**In ConfigureServices.cs:**
```csharp
services.Configure<AzureOpenAIOptions>(configuration.GetSection(nameof(AzureOpenAIOptions)));
services.AddSingleton<IOpenAIClientWrapper, OpenAIClientWrapper>();
services.AddScoped<IVehicleSearchService, VehicleSearchService>();
```

## Best Practices

### Prompt Engineering

1. **Be Specific**: Clearly state what you want to find
   ```
   \"Search for Toyota Camry 2020 for sale on automotive websites like Cars.com, AutoTrader\"
   ```

2. **Request Structured Output**: Ask for JSON format
   ```
   \"Return results as a JSON array with these fields: Make, Model, Year, Price, URL\"
   ```

3. **Set Context**: Use system messages effectively
   ```
   \"You are an assistant that finds vehicle listings. Always include actual URLs from search results.\"
   ```

### Cost Management

- **Use Appropriate Models**: GPT-3.5-Turbo is cheaper than GPT-4
- **Limit Output Tokens**: Set `MaxOutputTokenCount` appropriately
- **Cache Results**: Don't repeat identical searches
- **Monitor Usage**: Check Azure Portal for token consumption

### Error Handling

```csharp
try
{
    var response = await _openAIClient.GetChatCompletionWithGroundingAsync(deployment, prompt);
    return ParseResults(response);
}
catch (Azure.RequestFailedException ex)
{
    _logger.LogError(ex, \"Azure OpenAI request failed\");
    throw;
}
```

### Security

1. **Never commit API keys** to source control
2. Use **User Secrets** for local development:
   ```bash
   dotnet user-secrets set \"AzureOpenAIOptions:ApiKey\" \"your-key\"
   ```
3. Use **Azure Key Vault** for production
4. Use **Managed Identity** when deployed to Azure
5. Rotate keys regularly

## Pricing

### Pay-As-You-Go Pricing

Pricing varies by model and region. Example pricing (subject to change):

#### GPT-3.5-Turbo
- **Input**: ~$0.0015 per 1K tokens
- **Output**: ~$0.002 per 1K tokens

#### GPT-4
- **Input**: ~$0.03 per 1K tokens
- **Output**: ~$0.06 per 1K tokens

**Note**: Grounding with Bing Search may incur additional costs.

**Full Pricing Details:**  
https://azure.microsoft.com/en-us/pricing/details/cognitive-services/openai-service/

### Cost Optimization

- Start with GPT-3.5-Turbo for development
- Use GPT-4 only when needed for complex reasoning
- Set token limits to prevent runaway costs
- Implement caching for repeated queries
- Monitor usage through Azure Cost Management

## Advantages Over Direct Bing Search API

1. **AI-Powered Data Extraction**: LLM understands context and extracts structured data
2. **Flexibility**: Natural language prompts instead of rigid query syntax
3. **Better Parsing**: AI interprets unstructured web content
4. **Contextual Reasoning**: Can analyze if a price is \"good\" based on market data
5. **Consistent Output**: Generates well-formatted JSON responses
6. **Source Synthesis**: Combines information from multiple sources

## Additional Resources

### Articles & Blogs
- **\"Azure OpenAI Service - What is it?\"**  
  https://learn.microsoft.com/en-us/azure/ai-services/openai/overview
  
- **\"Grounding LLMs with Bing Search\"**  
  https://techcommunity.microsoft.com/blog/
  
- **\"Building Intelligent Search with Azure OpenAI\"**  
  https://dev.to/search?q=azure+openai

### Stack Overflow
- **Azure OpenAI Questions**  
  https://stackoverflow.com/questions/tagged/azure-openai

### Community Forums
- **Microsoft Q&A - Azure OpenAI**  
  https://learn.microsoft.com/en-us/answers/tags/387/azure-openai

## Troubleshooting

### Common Issues

1. **401 Unauthorized**
   - Check API key is correct
   - Verify endpoint URL matches your resource
   - Check key hasn't expired

2. **403 Forbidden**
   - Verify deployment name is correct
   - Check model is deployed in your resource
   - Verify subscription has access

3. **429 Too Many Requests**
   - Implement rate limiting
   - Check quota limits in Azure Portal
   - Consider upgrading tier

4. **Empty or Incorrect Results**
   - Refine your prompt for clarity
   - Ensure grounding is working (check response for citations)
   - Validate JSON parsing logic
   - Check token limits aren't truncating responses

5. **High Costs**
   - Monitor token usage
   - Use GPT-3.5-Turbo instead of GPT-4
   - Implement caching
   - Set MaxTokens limits

## Regions and Availability

Azure OpenAI is available in specific regions. Check availability:

**Region Availability:**  
https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models#model-summary-table-and-region-availability

**Recommended Regions:**
- East US
- West Europe
- UK South
- Japan East

## Support

For issues or questions:
- **Microsoft Support**: https://azure.microsoft.com/en-us/support/
- **Documentation**: https://learn.microsoft.com/en-us/azure/ai-services/openai/
- **GitHub Issues**: https://github.com/Azure-Samples/openai/issues
- **Community**: https://learn.microsoft.com/en-us/answers/tags/387/azure-openai

---

**Last Updated**: December 16, 2025  
**Autolynx Version**: 1.0  
**Azure OpenAI SDK Version**: 2.1.0

This document provides comprehensive information on integrating the Bing Web Search API for vehicle search functionality in Autolynx.

## Table of Contents
- [Overview](#overview)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Video Tutorials](#video-tutorials)
- [Code Examples](#code-examples)
- [Configuration](#configuration)
- [Best Practices](#best-practices)

## Overview

The **Bing Web Search API** is part of Microsoft Azure Cognitive Services that allows applications to send search queries and receive relevant web results in JSON format. For Autolynx, we use it to search across popular automotive websites for vehicle listings.

### Why Bing Search API?

- **Real-time Results**: Get current vehicle listings from live websites
- **Broad Coverage**: Search across multiple automotive platforms simultaneously
- **Cost-Effective**: Free tier available (1,000 transactions/month)
- **Reliable**: Enterprise-grade service from Microsoft Azure
- **Structured Data**: JSON responses easy to parse and process

## Getting Started

### 1. Create a Bing Search Resource in Azure

**Azure Portal Steps:**
1. Go to [Azure Portal](https://portal.azure.com)
2. Click "Create a resource"
3. Search for "Bing Search v7"
4. Click "Create"
5. Fill in the details:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or use existing
   - **Region**: Choose closest to your location
   - **Name**: Give it a unique name
   - **Pricing Tier**: Free (F1) or Standard (S1)
6. Click "Review + Create"
7. After deployment, go to resource and copy the **API Key** from "Keys and Endpoint"

**Detailed Guide:**
https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/create-bing-search-service-resource

### 2. Update Configuration

Add your API key to `appsettings.json`:

```json
{
  "BingSearchOptions": {
    "ApiKey": "your-actual-api-key-here",
    "Endpoint": "https://api.bing.microsoft.com/v7.0/search",
    "ResultCount": 10,
    "Market": "en-US"
  }
}
```

## API Documentation

### Official Microsoft Documentation

#### Core References
- **Bing Web Search API Overview**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/overview
  
- **API Reference & Endpoints**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/reference/endpoints
  
- **Query Parameters**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/reference/query-parameters
  
- **Response Objects**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/reference/response-objects

#### Quickstart Guides
- **C# Quickstart**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/quickstarts/rest/csharp
  
- **REST API Quickstart**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/quickstarts/rest/rest

#### Advanced Topics
- **Search Query Syntax**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/search-the-web
  
- **Filtering and Ranking Results**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/filter-answers
  
- **Paging Through Results**  
  https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/page-results

### API Endpoint

```
https://api.bing.microsoft.com/v7.0/search
```

### Request Format

**HTTP Method:** GET

**Required Headers:**
```
Ocp-Apim-Subscription-Key: YOUR_API_KEY
```

**Common Query Parameters:**

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `q` | string | **Required.** Search query | `Toyota Camry 2020 for sale` |
| `count` | integer | Number of results (default: 10, max: 50) | `10` |
| `offset` | integer | Offset for pagination | `0` |
| `mkt` | string | Market code | `en-US` |
| `safeSearch` | string | Filter adult content: Off, Moderate, Strict | `Moderate` |
| `freshness` | string | Filter by recency: Day, Week, Month | `Week` |

### Response Format

```json
{
  "webPages": {
    "value": [
      {
        "name": "2020 Toyota Camry for Sale",
        "url": "https://www.cars.com/...",
        "snippet": "Find 2020 Toyota Camry for sale. $25,000. 30,000 miles...",
        "displayUrl": "cars.com/...",
        "dateLastCrawled": "2025-12-15T10:00:00Z"
      }
    ],
    "totalEstimatedMatches": 1500000
  }
}
```

## Video Tutorials

### YouTube Resources

#### Beginner Tutorials
- **"Bing Search API for Beginners"**  
  https://www.youtube.com/results?search_query=bing+search+api+tutorial+beginners
  
- **"How to Use Microsoft Bing Search API"**  
  https://www.youtube.com/results?search_query=how+to+use+bing+search+api

#### C# Specific
- **"Bing Search API with C# .NET"**  
  https://www.youtube.com/results?search_query=bing+search+api+c%23+.net
  
- **"Azure Cognitive Services - Bing Search in C#"**  
  https://www.youtube.com/results?search_query=azure+bing+search+c%23+tutorial

#### Advanced Topics
- **"Building Search Applications with Bing API"**  
  https://www.youtube.com/results?search_query=building+search+app+bing+api
  
- **"Bing Custom Search API Tutorial"**  
  https://www.youtube.com/results?search_query=bing+custom+search+api+tutorial

### Microsoft Learn Video Courses
- **"Create a Web Search Application"**  
  https://learn.microsoft.com/en-us/training/modules/create-web-search-app/

## Code Examples

### Official Microsoft Samples

#### GitHub Repositories
- **Cognitive Services REST API Samples**  
  https://github.com/Azure-Samples/cognitive-services-REST-api-samples
  
- **Bing Search Samples for .NET**  
  https://github.com/Azure-Samples/cognitive-services-dotnet-sdk-samples

### Example Usage in Autolynx

#### Basic Search Request
```csharp
public async Task<string> SearchAsync(string query)
{
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
    
    var requestUri = $"https://api.bing.microsoft.com/v7.0/search?q={Uri.EscapeDataString(query)}&count=10";
    var response = await httpClient.GetAsync(requestUri);
    
    return await response.Content.ReadAsStringAsync();
}
```

#### Vehicle Search Query Construction
```csharp
var searchQuery = $"Toyota Camry 2020 for sale (site:cars.com OR site:autotrader.com OR site:cargurus.com)";
```

#### Parsing Results
```csharp
var bingResponse = JsonSerializer.Deserialize<BingSearchResponse>(jsonResponse);
foreach (var page in bingResponse.WebPages.Value)
{
    Console.WriteLine($"{page.Name}: {page.Url}");
}
```

## Configuration

### Autolynx Configuration Files

The application uses strongly-typed options for Bing Search configuration:

**BingSearchOptions.cs**
```csharp
public class BingSearchOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.bing.microsoft.com/v7.0/search";
    public int ResultCount { get; set; } = 10;
    public string Market { get; set; } = "en-US";
}
```

### Dependency Injection Setup

**In ConfigureServices.cs:**
```csharp
services.Configure<BingSearchOptions>(configuration.GetSection(nameof(BingSearchOptions)));
services.AddHttpClient<IBingSearchService, BingSearchService>();
```

## Best Practices

### Query Optimization

1. **Use Site Restrictions**: Limit searches to known automotive websites
   ```
   site:cars.com OR site:autotrader.com OR site:cargurus.com
   ```

2. **Include Specific Terms**: Add "for sale" to filter results
   ```
   Toyota Camry 2020 for sale
   ```

3. **Add Location**: Include city/state for localized results
   ```
   Toyota Camry 2020 for sale Los Angeles CA
   ```

### Error Handling

Always implement retry logic and proper exception handling:

```csharp
try
{
    var response = await _httpClient.GetAsync(requestUri);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Bing Search request failed");
    throw;
}
```

### Rate Limiting

- **Free Tier**: 3 queries per second, 1,000 per month
- **S1 Tier**: 10 queries per second, up to 1M per month
- Implement caching to reduce API calls

### Security

1. **Never commit API keys** to source control
2. Use **User Secrets** for local development:
   ```bash
   dotnet user-secrets set "BingSearchOptions:ApiKey" "your-key-here"
   ```
3. Use **Azure Key Vault** for production
4. Rotate keys regularly

## Pricing

### Free Tier (F1)
- **Price**: Free
- **Quota**: 1,000 transactions per month
- **Rate**: 3 queries per second
- **Best for**: Development and testing

### Standard Tier (S1)
- **Price**: $7 per 1,000 transactions
- **Quota**: Up to 1,000,000 transactions per month
- **Rate**: 10 queries per second
- **Best for**: Production applications

**Full Pricing Details:**  
https://www.microsoft.com/en-us/bing/apis/pricing

## Additional Resources

### Articles & Blogs
- **"Getting Started with Bing Search APIs"**  
  https://techcommunity.microsoft.com/blog/
  
- **"Bing Search API Best Practices"**  
  https://dev.to/search?q=bing%20search%20api
  
- **"Building Intelligent Search with Azure Cognitive Services"**  
  https://medium.com/search?q=bing+search+api

### Stack Overflow
- **Bing Search API Questions**  
  https://stackoverflow.com/questions/tagged/bing-search-api

### Community Forums
- **Microsoft Q&A - Bing Search**  
  https://learn.microsoft.com/en-us/answers/tags/134/azure-cognitive-search

## Troubleshooting

### Common Issues

1. **401 Unauthorized**
   - Check API key is correct
   - Verify key is in the header as `Ocp-Apim-Subscription-Key`

2. **403 Forbidden**
   - Check quota hasn't been exceeded
   - Verify subscription is active

3. **429 Too Many Requests**
   - Implement rate limiting
   - Add delays between requests

4. **Empty Results**
   - Verify query syntax
   - Check market parameter matches your region
   - Try broadening search terms

## Support

For issues or questions:
- **Microsoft Support**: https://azure.microsoft.com/en-us/support/
- **Documentation**: https://learn.microsoft.com/en-us/bing/search-apis/
- **GitHub Issues**: https://github.com/Azure-Samples/cognitive-services-REST-api-samples/issues

---

**Last Updated**: December 16, 2025  
**Autolynx Version**: 1.0
