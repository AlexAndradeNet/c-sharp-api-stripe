// <copyright file="CustomerResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace StripeAPITest.Main.Models;

public class CustomerResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("livemode")]
    public bool Livemode { get; set; }
}
