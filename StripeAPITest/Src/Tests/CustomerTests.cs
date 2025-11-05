// <copyright file="CustomerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using System.Text.Json;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
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
            await AllureApi.Step(
                $"Cleaning up {_createdCustomerIds.Count} customer(s)",
                async () =>
                {
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
            );
        }
    }

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

        CustomerResponse responseBody = null!;

        await AllureApi.Step(
            "Creating customer via Stripe API",
            async () =>
            {
                var response = await _stripeClient.CreateCustomer(customer);

                Assert.That(
                    response.Ok,
                    Is.True,
                    "Create customer should succeed"
                );

                responseBody =
                    await StripeApiClient.DeserializeResponse<CustomerResponse>(
                        response
                    );

                Assert.That(
                    response.Status,
                    Is.EqualTo(200),
                    "Expected status code 200"
                );
            }
        );

        AllureApi.Step(
            $"Response received for customer ID: {responseBody.Id}",
            () =>
            {
                // Ensure we have an ID before tracking the customer for cleanup
                Assert.That(
                    responseBody.Id,
                    Is.Not.Null.And.Not.Empty,
                    "Expected customer ID in response"
                );

                // Track the customer ID for cleanup
                _createdCustomerIds.Add(responseBody.Id!);

                LogCustomerCreated(Log, responseBody.Id!, null);
            }
        );

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

        CustomerResponse retrievedCustomer = null!;

        // Act - Get
        await AllureApi.Step(
            "Step 2: Retrieve customer",
            async () =>
            {
                var getResponse = await _stripeClient.GetCustomer(
                    createResponse.Id!
                );
                Assert.That(
                    getResponse.Ok,
                    Is.True,
                    "Get customer should succeed"
                );

                retrievedCustomer =
                    await StripeApiClient.DeserializeResponse<CustomerResponse>(
                        getResponse
                    );
            }
        );

        // Assert
        AllureApi.Step(
            "Step 3: Verify data consistency",
            () =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        retrievedCustomer.Id,
                        Is.EqualTo(createResponse.Id)
                    );
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
        );
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

        var updatedCustomer = TestDataGenerator.RandomCustomerWithPrefix(
            "Updated"
        );
        AllureApi.Step(
            $"Step 2: Generated customer: {updatedCustomer.Name} - {updatedCustomer.Email}"
        );

        // Act
        CustomerResponse updated = null!;
        await AllureApi.Step(
            "Step 3: Update customer",
            async () =>
            {
                var updateResponse = await _stripeClient.UpdateCustomer(
                    createResponse.Id!,
                    updatedCustomer
                );

                // Assert
                Assert.That(
                    updateResponse.Ok,
                    Is.True,
                    "Update should succeed"
                );

                updated =
                    await StripeApiClient.DeserializeResponse<CustomerResponse>(
                        updateResponse
                    );
            }
        );

        AllureApi.Step(
            "Step 4: Verify data consistency",
            () =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(updated.Name, Is.EqualTo(updatedCustomer.Name));
                    Assert.That(
                        updated.Email,
                        Is.EqualTo(updatedCustomer.Email)
                    );
                });
            }
        );
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
        IAPIResponse deleteResponse = null!;
        await AllureApi.Step(
            $"Step 2: Delete customer: {createResponse.Name} - {createResponse.Email}",
            async () =>
            {
                deleteResponse = await _stripeClient.DeleteCustomer(
                    createResponse.Id!
                );
            }
        );

        // Assert
        AllureApi.Step(
            $"Step 3: Verify deletion: {createResponse.Name} - {createResponse.Email}",
            () =>
            {
                Assert.That(
                    deleteResponse.Ok,
                    Is.True,
                    "Delete should succeed"
                );
                LogCustomerDeleted(Log, createResponse.Id!, null);

                // Prevent the unnecessary AfterEach cleaning
                _createdCustomerIds.Remove(createResponse.Id!);
            }
        );
    }

    [Test]
    [AllureTag("API", "Customer", "Create", "Get")]
    [AllureSeverity(SeverityLevel.critical)]
    [TestCase(TestName = "Test getting multiple customer via Stripe API")]
    [AllureOwner("QA Team")]
    public async Task ListCustomersShouldReturnCustomers()
    {
        // Act
        CustomerListResponse responseUserListBody = null!;
        await AllureApi.Step(
            "Step 1: Fetching list of customers (limit: 5)",
            async () =>
            {
                var responseUserList = await _stripeClient.ListCustomers(5);

                // Assert
                Assert.That(
                    responseUserList.Ok,
                    Is.True,
                    "List customers should succeed"
                );

                responseUserListBody =
                    await StripeApiClient.DeserializeResponse<CustomerListResponse>(
                        responseUserList
                    );
            }
        );

        AllureApi.Step(
            "Step 2: Verify list of customers (limit: 5)",
            () =>
            {
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
                Assert.That(
                    responseUserListBody.Data,
                    Has.Count.EqualTo(5),
                    "Customer list count should be less than or equal to 5"
                );

                AllureApi.AddAttachment(
                    "Customer List",
                    "application/json",
                    Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(responseUserListBody)
                    )
                );
            }
        );
    }
}
