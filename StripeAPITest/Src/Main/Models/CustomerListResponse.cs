// <copyright file="CustomerListResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace StripeAPITest.Main.Models;

public class CustomerListResponse
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("data")]
    public List<CustomerResponse>? Data { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
