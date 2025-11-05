# Stripe API Test Suite

## Project Structure

```
StripeAPITest/
├── Config/
│   └── ApiConfig.cs          # Configuration management
├── Clients/
│   ├── BaseApiClient.cs      # Base API client with common functionality
│   └── StripeApiClient.cs    # Stripe-specific API methods
├── Models/
│   ├── CustomerRequest.cs    # Request DTOs
│   └── CustomerResponse.cs   # Response DTOs
├── Tests/
│   └── CustomerTests.cs      # Test cases
├── .env.example              # Environment variables template
└── StripeAPITest.csproj      # Project file
```

## Setup Instructions

1. **Install dependencies**
   ```bash
   dotnet restore
   ```

2. **Install Playwright browsers**
   ```bash
   pwsh bin/Debug/net8.0/playwright.ps1 install
   ```

3. **Configure environment variables**
    - Copy `.env.example` to `.env`
    - Add your Stripe API key:
      ```
      STRIPE_BASE_URL=https://api.stripe.com
      STRIPE_API_KEY=sk_test_your_actual_key_here
      ```

4. **Run tests**
   ```bash
   dotnet test
   ```

## API Client Pattern Benefits

- **Separation of Concerns**: API logic separated from test logic
- **Reusability**: Client can be used across multiple test files
- **Maintainability**: Changes to API endpoints only require updates in one
  place
- **Testability**: Easy to mock or stub the client for unit tests
- **Type Safety**: Strong typing with request/response models
- **Configuration Management**: Centralized config with environment variables

## Test Examples

The suite includes tests for:

- Creating customers
- Retrieving customers
- Updating customer data
- Deleting customers
- Listing customers

## Security Notes

- Never commit `.env` file with real API keys
- Use test mode API keys (starting with `sk_test_`)
- Add `.env` to `.gitignore`
