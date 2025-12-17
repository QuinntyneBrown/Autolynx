# Bing Search API Integration Guide

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
