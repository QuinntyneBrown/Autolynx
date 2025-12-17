# Migration Guide: Bing Search to Azure OpenAI with Grounding

This document explains the migration from direct Bing Search API integration to Azure OpenAI with Grounding.

## What Changed

### Previous Implementation (Obsolete)
The application previously used the **Bing Web Search API** directly to search for vehicle listings. This approach:
- Made direct HTTP calls to Bing Search API
- Required manual parsing of search results with regex
- Had limited contextual understanding
- Needed explicit data extraction logic

### New Implementation (Current)
The application now uses **Azure OpenAI Service with Grounding**, which:
- Leverages GPT models with built-in web search capabilities
- Uses AI to interpret and structure unstructured web data
- Provides contextual analysis (e.g., price competitiveness)
- Generates consistent, structured JSON outputs
- Includes source citations and attributions

## Why the Change

### Bing Search API Limitations
1. **Manual Data Extraction**: Required regex patterns to extract vehicle details
2. **Inconsistent Results**: Web page formats vary across automotive sites
3. **Limited Context**: Couldn't determine if prices were "good deals"
4. **Rigid Queries**: Needed specific search syntax
5. **Maintenance Overhead**: Parsing logic broke when websites changed

### Azure OpenAI with Grounding Advantages
1. **AI-Powered Interpretation**: GPT models understand unstructured content
2. **Contextual Reasoning**: Can analyze if prices are competitive
3. **Flexible Prompts**: Natural language instead of strict query syntax
4. **Consistent Output**: AI formats data uniformly
5. **Future-Proof**: AI adapts to website changes automatically

## Technical Changes

### Removed Files
- `BingSearchService.cs`
- `IBingSearchService.cs`
- `BingSearchOptions.cs`
- `BingSearchResult.cs`

### Modified Files
- `VehicleSearchService.cs` - Now uses `IOpenAIClientWrapper`
- `OpenAIClientWrapper.cs` - Added `GetChatCompletionWithGroundingAsync()` method
- `IOpenAIClientWrapper.cs` - Interface updated with grounding method
- `appsettings.json` - Changed from `BingSearchOptions` to `AzureOpenAIOptions`
- `ConfigureServices.cs` - Updated DI registration

### New Configuration

**Before (Bing Search):**
```json
{
  "BingSearchOptions": {
    "ApiKey": "bing-search-key",
    "Endpoint": "https://api.bing.microsoft.com/v7.0/search",
    "ResultCount": 10,
    "Market": "en-US"
  }
}
```

**After (Azure OpenAI):**
```json
{
  "AzureOpenAIOptions": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-openai-key",
    "DeploymentName": "gpt-4"
  }
}
```

## Migration Steps

### 1. Create Azure OpenAI Resource

1. Go to [Azure Portal](https://portal.azure.com)
2. Create an "Azure OpenAI" resource
3. Deploy a model:
   - Recommended: GPT-4 or GPT-3.5-Turbo
   - Region: Choose one with model availability
4. Note your:
   - Endpoint URL
   - API Key
   - Deployment Name

### 2. Update Configuration Files

**API (src/Autolynx.Api/appsettings.json):**
```json
{
  "AzureOpenAIOptions": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "your-deployment-name"
  }
}
```

**CLI (src/Autolynx.Cli/appsettings.json):**
```json
{
  "AzureOpenAIOptions": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "your-deployment-name"
  }
}
```

### 3. Update User Secrets (Development)

```bash
cd src/Autolynx.Api
dotnet user-secrets set "AzureOpenAIOptions:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAIOptions:ApiKey" "your-api-key"
dotnet user-secrets set "AzureOpenAIOptions:DeploymentName" "gpt-4"

cd ../Autolynx.Cli
dotnet user-secrets set "AzureOpenAIOptions:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAIOptions:ApiKey" "your-api-key"
dotnet user-secrets set "AzureOpenAIOptions:DeploymentName" "gpt-4"
```

### 4. Update Production Configuration

For Azure App Service or other production environments:
- Use Azure Key Vault for secrets
- Set environment variables:
  - `AzureOpenAIOptions__Endpoint`
  - `AzureOpenAIOptions__ApiKey`
  - `AzureOpenAIOptions__DeploymentName`
- Or use Managed Identity for authentication

### 5. Test the Migration

```bash
# Build the solution
dotnet build Autolynx.sln

# Run tests
dotnet test Autolynx.sln

# Test the CLI
cd src/Autolynx.Cli
dotnet run

# Test the API
cd ../Autolynx.Api
dotnet run
```

## Cost Comparison

### Bing Search API
- **Free Tier**: 1,000 transactions/month
- **Paid Tier**: $7 per 1,000 transactions
- **Cost**: Predictable per-search pricing

### Azure OpenAI with Grounding
- **Pricing**: Based on tokens (input + output)
- **GPT-3.5-Turbo**: ~$0.0015-0.002 per 1K tokens
- **GPT-4**: ~$0.03-0.06 per 1K tokens
- **Variable Cost**: Depends on prompt length and response size

**Cost Optimization Tips:**
1. Start with GPT-3.5-Turbo (cheaper)
2. Set `MaxOutputTokenCount` limits
3. Cache frequent queries
4. Use GPT-4 only when needed

## Grounding vs. Direct Bing Search

### How Grounding Works

Azure OpenAI's grounding feature:
1. Takes your prompt
2. Internally performs web search (using Bing)
3. Retrieves relevant web pages
4. Feeds content to the GPT model
5. Model generates response based on search results
6. Includes citations to sources

### No Separate Bing API Key Needed

Unlike the old implementation, you **do not** need a separate Bing Search API subscription. Grounding is built into Azure OpenAI Service.

## Troubleshooting

### "Grounding not working" or "No web results"

**Solution**: Ensure your prompt explicitly requests web search:
```csharp
var prompt = "Use current web search to find Toyota Camry 2020 for sale...";
```

### "Response doesn't include citations"

**Solution**: The model may not always include citations. Update your prompt:
```csharp
var prompt = "Search the web and include source URLs for each result...";
```

### "High costs compared to Bing Search"

**Solutions**:
1. Use GPT-3.5-Turbo instead of GPT-4
2. Reduce `MaxOutputTokenCount`
3. Implement response caching
4. Optimize prompt length

### "Model not available in my region"

**Solution**: Azure OpenAI availability varies by region. Check:
https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models#model-summary-table-and-region-availability

Deploy your resource in a supported region (e.g., East US, West Europe).

## Rollback Plan

If you need to revert to Bing Search:

1. Checkout the previous commit before migration:
   ```bash
   git log --oneline  # Find the commit before migration
   git checkout <commit-hash>
   ```

2. Restore Bing Search configuration in appsettings.json

3. Rebuild the solution

**Note**: The Azure OpenAI implementation is recommended for better results and future-proofing.

## Support Resources

- **Azure OpenAI Documentation**: https://learn.microsoft.com/en-us/azure/ai-services/openai/
- **Grounding Guide**: See [AZURE-OPENAI-GROUNDING.md](AZURE-OPENAI-GROUNDING.md)
- **Azure Support**: https://azure.microsoft.com/en-us/support/

## Summary

The migration from Bing Search to Azure OpenAI with Grounding provides:
- ✅ Better data extraction through AI
- ✅ Contextual understanding and analysis
- ✅ More consistent, structured outputs
- ✅ Future-proof architecture
- ✅ Reduced maintenance overhead
- ⚠️ Variable costs (monitor token usage)
- ⚠️ Requires Azure OpenAI resource

The new implementation leverages modern AI capabilities to deliver superior results with less manual data processing.

---

**Migration Date**: December 16, 2025  
**Autolynx Version**: 1.0  
**Status**: Complete
