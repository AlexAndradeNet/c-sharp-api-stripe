// <copyright file="BaseApiClient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json;
using Microsoft.Playwright;
using StripeAPITest.Main.Utils.Logger;

namespace StripeAPITest.Main.Clients;

public abstract class BaseApiClient : IDisposable, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonReadOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _baseUrl;

    private readonly Dictionary<string, string> _defaultHeaders;
    private bool _disposed;
    private IPlaywright? _playwright;

    protected BaseApiClient(string baseUrl, Dictionary<string, string> headers)
    {
        _baseUrl = baseUrl;
        _defaultHeaders = headers;
        InitializeAsync(baseUrl, headers).GetAwaiter().GetResult();
    }

    private IAPIRequestContext? ApiContext { get; set; }

    // Non-null accessor that throws when the API context hasn't been initialized.
    private IAPIRequestContext Context =>
        ApiContext
        ?? throw new InvalidOperationException(
            "API request context is not initialized."
        );

    // Provide an async disposal method per IAsyncDisposable using the recommended pattern.
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (ApiContext != null)
        {
            await ApiContext.DisposeAsync();
            ApiContext = null;
        }

        // Playwright only offers a synchronous Dispose.
        _playwright?.Dispose();
        _playwright = null;
    }

    // Public IAsyncDisposable implementation
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await DisposeAsyncCore();

        // Match sync dispose pattern: release unmanaged resources and mark disposed.
        Dispose(false);

        GC.SuppressFinalize(this);
    }

    // Public synchronous Dispose - keeps compatibility with consumers that use IDisposable.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Finalizer (only needed if unmanaged resources exist).
    ~BaseApiClient()
    {
        Dispose(false);
    }

    protected static async Task<T> DeserializeResponse<T>(IAPIResponse response)
    {
        var content = await response.TextAsync();
        return JsonSerializer.Deserialize<T>(content, JsonReadOptions)
            ?? throw new InvalidOperationException(
                "Failed to deserialize response"
            );
    }

    protected async Task<IAPIResponse> GetAsync(
        string url,
        APIRequestContextOptions? options = null
    )
    {
        var headers = MergeHeaders(options?.Headers);
        RequestLogger.LogRequest("GET", url, _baseUrl, headers);

        var response = await Context.GetAsync(url, options);
        await RequestLogger.LogResponse(response);

        return response;
    }

    protected async Task<IAPIResponse> PostAsync(
        string url,
        APIRequestContextOptions? options = null
    )
    {
        var headers = MergeHeaders(options?.Headers);
        var data = ExtractData(options);

        RequestLogger.LogRequest("POST", url, _baseUrl, headers, data);

        var response = await Context.PostAsync(url, options);
        await RequestLogger.LogResponse(response);

        return response;
    }

    protected async Task<IAPIResponse> PutAsync(
        string url,
        APIRequestContextOptions? options = null
    )
    {
        var headers = MergeHeaders(options?.Headers);
        var data = ExtractData(options);

        RequestLogger.LogRequest("PUT", url, _baseUrl, headers, data);

        var response = await Context.PutAsync(url, options);
        await RequestLogger.LogResponse(response);

        return response;
    }

    protected async Task<IAPIResponse> PatchAsync(
        string url,
        APIRequestContextOptions? options = null
    )
    {
        var headers = MergeHeaders(options?.Headers);
        var data = ExtractData(options);

        RequestLogger.LogRequest("PATCH", url, _baseUrl, headers, data);

        var response = await Context.PatchAsync(url, options);
        await RequestLogger.LogResponse(response);

        return response;
    }

    protected async Task<IAPIResponse> DeleteAsync(
        string url,
        APIRequestContextOptions? options = null
    )
    {
        var headers = MergeHeaders(options?.Headers);
        RequestLogger.LogRequest("DELETE", url, _baseUrl, headers);

        var response = await Context.DeleteAsync(url, options);
        await RequestLogger.LogResponse(response);

        return response;
    }

    protected static string DictionaryToFormData(
        Dictionary<string, string> data
    )
    {
        return string.Join(
            "&",
            data.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"
            )
        );
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources synchronously.
            // If ApiContext is present, ensure we block safely on its ValueTask by converting to Task.
            if (ApiContext != null)
            {
                ApiContext.DisposeAsync().AsTask().GetAwaiter().GetResult();
                ApiContext = null;
            }

            _playwright?.Dispose();
            _playwright = null;
        }

        // Release unmanaged resources here if any.

        _disposed = true;
    }

    private Dictionary<string, string> MergeHeaders(
        IEnumerable<KeyValuePair<string, string>>? additionalHeaders
    )
    {
        var merged = new Dictionary<string, string>(_defaultHeaders);

        if (additionalHeaders == null)
            return merged;
        foreach (var header in additionalHeaders)
            merged[header.Key] = header.Value;

        return merged;
    }

    private static object? ExtractData(APIRequestContextOptions? options)
    {
        if (options?.Data == null)
            return options?.Form;
        if (options.Data is { } stringData)
            return stringData;

        return options.Data;
    }

    private async Task InitializeAsync(
        string url,
        Dictionary<string, string> headers
    )
    {
        _playwright = await Playwright.CreateAsync();
        ApiContext = await _playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions
            {
                BaseURL = url,
                ExtraHTTPHeaders = headers,
            }
        );
    }
}
