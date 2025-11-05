using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace StripeAPITest.Main.Utils.Logger;

public static class CustomLoggerFactory
{
    private static readonly Lazy<ILoggerFactory> LazyFactory = new(() =>
    {
        return LoggerFactory.Create(builder =>
        {
            builder.AddNLog(); // Now this will work
        });
    });

    private static ILoggerFactory Factory => LazyFactory.Value;

    public static ILogger CreateLogger(string categoryName)
    {
        return Factory.CreateLogger(categoryName);
    }

    public static void DisposeFactory()
    {
        if (LazyFactory.IsValueCreated)
            LazyFactory.Value.Dispose();
    }
}
