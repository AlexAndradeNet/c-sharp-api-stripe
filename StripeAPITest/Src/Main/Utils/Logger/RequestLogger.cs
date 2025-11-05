// <copyright file="RequestLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using Allure.Net.Commons;
using Microsoft.Playwright;

namespace StripeAPITest.Main.Utils.Logger;

public static class RequestLogger
{
    public static void LogRequest(
        string method,
        string url,
        string? baseUrl = null,
        Dictionary<string, string>? headers = null,
        object? data = null
    )
    {
        var sb = new StringBuilder();
        var separator = new string('=', 80);
        var fullUrl = string.IsNullOrEmpty(baseUrl) ? url : $"{baseUrl}{url}";

        sb.AppendLine(separator);
        sb.AppendLine($"REQUEST: {method} {fullUrl}");
        sb.AppendLine($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine(separator);

        // Log headers
        if (headers != null && headers.Count != 0)
        {
            sb.AppendLine("Headers:");
            foreach (var header in headers)
            {
                // Mask sensitive headers
                var value = IsSensitiveHeader(header.Key)
                    ? MaskSensitiveValue(header.Value)
                    : header.Value;
                sb.AppendLine($"  {header.Key}: {value}");
            }

            sb.AppendLine();
        }

        // Log request body/data
        if (data != null)
        {
            sb.AppendLine("Request Body:");
            switch (data)
            {
                case string stringData:
                    sb.AppendLine($"  {stringData}");
                    break;
                case Dictionary<string, string> formData:
                {
                    foreach (var item in formData)
                        sb.AppendLine($"  {item.Key}: {item.Value}");
                    break;
                }
                default:
                    sb.AppendLine($"  {data}");
                    break;
            }

            sb.AppendLine();
        }

        sb.AppendLine(separator);
        sb.AppendLine();

        LogToAllure("Request", sb);
    }

    public static async Task LogResponse(IAPIResponse response)
    {
        var sb = new StringBuilder();
        var separator = new string('-', 80);

        sb.AppendLine(separator);
        sb.AppendLine($"RESPONSE: {response.Status} {response.StatusText}");
        sb.AppendLine($"URL: {response.Url}");
        sb.AppendLine(separator);

        // Log response headers
        var responseHeaders = response.Headers;
        if (responseHeaders.Count != 0)
        {
            sb.AppendLine("Response Headers:");
            foreach (var header in responseHeaders)
                sb.AppendLine($"  {header.Key}: {header.Value}");
            sb.AppendLine();
        }

        sb.AppendLine(separator);
        sb.AppendLine("BODY:");
        sb.AppendLine(await response.TextAsync());
        sb.AppendLine();

        LogToAllure("Response", sb);
    }

    private static bool IsSensitiveHeader(string headerName)
    {
        var sensitiveHeaders = new[]
        {
            "authorization",
            "api-key",
            "x-api-key",
            "bearer",
            "token",
            "password",
            "secret",
        };

        return sensitiveHeaders.Any(h =>
            headerName.Contains(h, StringComparison.OrdinalIgnoreCase)
        );
    }

    private static string MaskSensitiveValue(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= 8)
            return "***MASKED***";

        // Show first 4 and last 4 characters
        return $"{value.Substring(0, 4)}...{value.Substring(value.Length - 4)}";
    }

    private static void LogToAllure(string title, StringBuilder value)
    {
        AllureApi.AddAttachment(
            title,
            "text/plain",
            Encoding.UTF8.GetBytes(value.ToString())
        );
    }
}
