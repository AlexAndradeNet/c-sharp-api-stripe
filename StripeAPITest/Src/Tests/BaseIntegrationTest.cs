// <copyright file="BaseIntegrationTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StripeAPITest.Main.Utils.Logger;

namespace StripeAPITest.Tests;

[AllureNUnit]
[AllureSuite("Stripe API Integration Tests")]
[AllureDisplayIgnored]
public abstract partial class BaseIntegrationTest // 1. Make the class 'partial'
{
    // Each test class will get its own logger instance
    // 2. Added '= null!' to fix the CS8618 nullability warning
    protected ILogger Log { get; private set; } = null!;

    [OneTimeSetUp]
    [AllureBefore]
    public void BaseOneTimeSetup()
    {
        // We get a logger using the logger's type as the category
        Log = CustomLoggerFactory.CreateLogger(GetType().Name);

        // 3. Call the new, generated method instead
        LogFixtureInitialized(Log);
    }

    // This ensures all logs are flushed when the test run is done
    [OneTimeTearDown]
    [AllureAfter]
    public static void BaseOneTimeTearDown()
    {
        CustomLoggerFactory.DisposeFactory();
    }

    // 4. Define the static partial method for the source generator
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 1, // It's good practice to give it an ID
        Message = "--- Logger Initialized for Test Fixture ---"
    )]
    private static partial void LogFixtureInitialized(ILogger logger);
}
