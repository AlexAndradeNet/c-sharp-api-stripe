// <copyright file="CustomerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using System.Text.Json;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StripeAPITest.Main.Clients;
using StripeAPITest.Main.Models;
using StripeAPITest.Main.Utils;

namespace StripeAPITest.Tests;

[TestFixture]
[AllureFeature("Stripe API")]
public class CustomerTests : BaseIntegrationTest
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _stripeClient = new StripeApiClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _stripeClient.Dispose();
    }

    [TearDown]
    [AllureAfter]
    public async Task TearDown()
    {
        // Clean up all created customers after each test
        if (_createdCustomerIds.Count > 0)
        {
            AllureApi.Step(
                $"Cleaning up {_createdCustomerIds.Count} customer(s)"
            );

            foreach (var customerId in _createdCustomerIds)
                try
                {
                    await _stripeClient.DeleteCustomer(customerId);
                    LogCleanedUpCustomer(Log, customerId, null);
                }
                catch (Exception ex)
                {
                    LogFailedToDeleteCustomer(
                        Log,
                        customerId,
                        ex.Message,
                        null
                    );
                }

            _createdCustomerIds.Clear();
        }
    }

    // Predefined logger message delegates for improved performance
    private static readonly Action<
        ILogger,
        string,
        Exception?
    > LogCleanedUpCustomer = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(1001, nameof(LogCleanedUpCustomer)),
        "🧹 Cleaned up customer: {CustomerId}"
    );

    private static readonly Action<
        ILogger,
        string,
        Exception?
    > LogCustomerCreated = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(1002, nameof(LogCustomerCreated)),
        "✔️ Customer created: {CustomerId}"
    );

    private static readonly Action<
        ILogger,
        string,
        string,
        Exception?
    > LogFailedToDeleteCustomer = LoggerMessage.Define<string, string>(
        LogLevel.Warning,
        new EventId(1003, nameof(LogFailedToDeleteCustomer)),
        "⚠️ Failed to delete customer {CustomerId}: {Error}"
    );

    private static readonly Action<
        ILogger,
        string,
        Exception?
    > LogCustomerDeleted = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(1004, nameof(LogCustomerDeleted)),
        "✔️ Deleted customer: {CustomerId}"
    );

    private StripeApiClient _stripeClient = null!;

    private readonly List<string> _createdCustomerIds = []; // Track created customers

    // Helper method to track customer creation
    [AllureStep("Given the user is created")]
    private async Task<CustomerResponse> CreateAndTrackCustomer()
    {
        var customer = TestDataGenerator.RandomCustomer();
        AllureApi.Step(
            $"Generated customer: {customer.Name} - {customer.Email}"
        );

        AllureApi.Step("Creating customer via Stripe API");
        var response = await _stripeClient.CreateCustomer(customer);

        Assert.That(response.Ok, Is.True, "Create customer should succeed");

        var responseBody =
            await StripeApiClient.DeserializeResponse<CustomerResponse>(
                response
            );

        Assert.That(
            response.Status,
            Is.EqualTo(200),
            "Expected status code 200"
        );

        AllureApi.Step($"Response received for customer ID: {responseBody.Id}");

        // Ensure we have an ID before tracking the customer for cleanup
        Assert.That(
            responseBody.Id,
            Is.Not.Null.And.Not.Empty,
            "Expected customer ID in response"
        );

        // Track the customer ID for cleanup
        _createdCustomerIds.Add(responseBody.Id!);

        LogCustomerCreated(Log, responseBody.Id!, null);

        return responseBody;
    }

    [Test]
    [AllureTag("API", "Customer", "Create")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureName("Test creating a new customer via Stripe API with random data")]
    [AllureOwner("QA Team")]
    public async Task CreateCustomerShouldSucceed()
    {
        // Arrange
        await CreateAndTrackCustomer();
    }

    [Test]
    [AllureTag("API", "Customer", "Create", "Get")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription(
        "Test creating a new customer via Stripe API with random data"
    )]
    [AllureOwner("QA Team")]
    public async Task CreateAndGetCustomerShouldReturnSameData()
    {
        // Arrange
        var createResponse = await CreateAndTrackCustomer();

        // Act - Get
        AllureApi.Step("Step 2: Retrieve customer");
        var getResponse = await _stripeClient.GetCustomer(createResponse.Id!);
        Assert.That(getResponse.Ok, Is.True, "Get customer should succeed");

        var retrievedCustomer =
            await StripeApiClient.DeserializeResponse<CustomerResponse>(
                getResponse
            );

        // Assert
        AllureApi.Step("Step 3: Verify data consistency");
        Assert.Multiple(() =>
        {
            Assert.That(retrievedCustomer.Id, Is.EqualTo(createResponse.Id));
            Assert.That(
                retrievedCustomer.Name,
                Is.EqualTo(createResponse.Name)
            );
            Assert.That(
                retrievedCustomer.Email,
                Is.EqualTo(createResponse.Email)
            );
        });
    }

    [Test]
    [AllureTag("API", "Customer", "Create", "Get", "Modify")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription(
        "Test creating and modify a new customer via Stripe API with random data"
    )]
    [AllureOwner("QA Team")]
    public async Task UpdateCustomerShouldModifyCreateResponse()
    {
        // Arrange
        var createResponse = await CreateAndTrackCustomer();

        // Act
        var updatedCustomer = TestDataGenerator.RandomCustomerWithPrefix(
            "Updated"
        );
        var updateResponse = await _stripeClient.UpdateCustomer(
            createResponse.Id!,
            updatedCustomer
        );

        // Assert
        Assert.That(updateResponse.Ok, Is.True, "Update should succeed");
        var updated =
            await StripeApiClient.DeserializeResponse<CustomerResponse>(
                updateResponse
            );
        Assert.Multiple(() =>
        {
            Assert.That(updated.Name, Is.EqualTo(updatedCustomer.Name));
            Assert.That(updated.Email, Is.EqualTo(updatedCustomer.Email));
        });
    }

    [Test]
    [AllureTag("API", "Customer", "Create", "Delete")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription(
        "Test creating and delete a new customer via Stripe API with random data"
    )]
    [AllureOwner("QA Team")]
    public async Task DeleteCustomerShouldRemoveCustomer()
    {
        // Arrange
        var createResponse = await CreateAndTrackCustomer();

        // Act
        var deleteResponse = await _stripeClient.DeleteCustomer(
            createResponse.Id!
        );

        // Assert
        Assert.That(deleteResponse.Ok, Is.True, "Delete should succeed");
        LogCustomerDeleted(Log, createResponse.Id!, null);

        // Prevent the unnecessary AfterEach cleaning
        _createdCustomerIds.Remove(createResponse.Id!);
    }

    [Test]
    [AllureTag("API", "Customer", "Create", "Get")]
    [AllureSeverity(SeverityLevel.critical)]
    [TestCase(TestName = "Test getting multiple customer via Stripe API")]
    [AllureOwner("QA Team")]
    public async Task ListCustomersShouldReturnCustomers()
    {
        // Act
        AllureApi.Step("Fetching list of customers (limit: 5)");
        var responseUserList = await _stripeClient.ListCustomers(5);

        // Assert
        Assert.That(
            responseUserList.Ok,
            Is.True,
            "List customers should succeed"
        );

        var responseUserListBody =
            await StripeApiClient.DeserializeResponse<CustomerListResponse>(
                responseUserList
            );

        Assert.That(
            responseUserListBody.Data,
            Is.Not.Null,
            "Customer list data should not be null"
        );
        Assert.That(
            responseUserListBody.Data,
            Is.Not.Empty,
            "Customer list should not be empty"
        );

        AllureApi.AddAttachment(
            "Customer List",
            "application/json",
            Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(responseUserListBody)
            )
        );

        AllureApi.Step("✓ Successfully retrieved customer list");
    }
}
