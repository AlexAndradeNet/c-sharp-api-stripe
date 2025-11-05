// <copyright file="ApiConfig.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DotNetEnv;

namespace StripeAPITest.Main.Config;

public static class ApiConfig
{
    static ApiConfig()
    {
        // Load .env file from project root
        Env.Load();
    }

    public static string StripeBaseUrl =>
        Environment.GetEnvironmentVariable("STRIPE_BASE_URL")
        ?? "https://api.stripe.com";

    public static string StripeApiKey =>
        Environment.GetEnvironmentVariable("STRIPE_API_KEY")
        ?? throw new InvalidOperationException(
            "STRIPE_API_KEY not found in environment variables"
        );
}
