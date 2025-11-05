// <copyright file="StripeApiClient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using Microsoft.Playwright;
using StripeAPITest.Main.Config;
using StripeAPITest.Main.Models;

namespace StripeAPITest.Main.Clients;

public class StripeApiClient()
    : BaseApiClient(ApiConfig.StripeBaseUrl, DefaultHeaders)
{
    // Initialize static field inline to satisfy analyzer and simplify initialization.
    private static readonly Dictionary<string, string> DefaultHeaders =
        CreateDefaultHeaders();

    private static Dictionary<string, string> CreateDefaultHeaders()
    {
        var apiKey = ApiConfig.StripeApiKey;
        var base64Auth = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{apiKey}:")
        );

        return new Dictionary<string, string>
        {
            { "Authorization", $"Basic {base64Auth}" },
        };
    }

    public async Task<IAPIResponse> CreateCustomer(CustomerRequest customer)
    {
        var formData = new Dictionary<string, string>
        {
            { "name", customer.Name },
            { "email", customer.Email },
        };

        var response = await PostAsync(
            "/v1/customers",
            new APIRequestContextOptions
            {
                Data = DictionaryToFormData(formData),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                },
            }
        );

        return response;
    }

    public async Task<IAPIResponse> GetCustomer(string customerId)
    {
        var response = await GetAsync($"/v1/customers/{customerId}");

        return response;
    }

    public async Task<IAPIResponse> UpdateCustomer(
        string customerId,
        CustomerRequest customer
    )
    {
        var formData = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(customer.Name))
            formData.Add("name", customer.Name);

        if (!string.IsNullOrEmpty(customer.Email))
            formData.Add("email", customer.Email);

        var response = await PostAsync(
            $"/v1/customers/{customerId}",
            new APIRequestContextOptions
            {
                Data = DictionaryToFormData(formData),
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                },
            }
        );

        return response;
    }

    public async Task<IAPIResponse> DeleteCustomer(string customerId)
    {
        var response = await DeleteAsync($"/v1/customers/{customerId}");

        return response;
    }

    public async Task<IAPIResponse> ListCustomers(int limit = 10)
    {
        var response = await GetAsync($"/v1/customers?limit={limit}");

        return response;
    }

    public static new async Task<T> DeserializeResponse<T>(
        IAPIResponse response
    )
    {
        return await BaseApiClient.DeserializeResponse<T>(response);
    }
}
