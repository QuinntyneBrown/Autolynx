// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json.Serialization;

namespace Autolynx.Core.Models;

public class BingSearchResponse
{
    [JsonPropertyName("webPages")]
    public WebPages? WebPages { get; set; }
}

public class WebPages
{
    [JsonPropertyName("value")]
    public List<WebPage> Value { get; set; } = new();

    [JsonPropertyName("totalEstimatedMatches")]
    public long TotalEstimatedMatches { get; set; }
}

public class WebPage
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("snippet")]
    public string Snippet { get; set; } = string.Empty;

    [JsonPropertyName("displayUrl")]
    public string DisplayUrl { get; set; } = string.Empty;

    [JsonPropertyName("dateLastCrawled")]
    public DateTime? DateLastCrawled { get; set; }
}
