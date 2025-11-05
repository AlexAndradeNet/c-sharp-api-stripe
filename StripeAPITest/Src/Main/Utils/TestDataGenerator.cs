using Bogus;
using StripeAPITest.Main.Models;

namespace StripeAPITest.Main.Utils;

public static class TestDataGenerator
{
    private static readonly Faker Faker = new();

    // Generate a single random customer
    public static CustomerRequest RandomCustomer()
    {
        return new CustomerRequest(
            Faker.Name.FullName(),
            Faker.Internet.Email()
        );
    }

    // Generate a customer with specific name format
    public static CustomerRequest RandomCustomerWithPrefix(string prefix)
    {
        return new CustomerRequest(
            $"{prefix} {Faker.Name.LastName()}",
            Faker.Internet.Email()
        );
    }

    // Generate multiple random customers
    public static List<CustomerRequest> RandomCustomers(int count)
    {
        var customerFaker = new Faker<CustomerRequest>().CustomInstantiator(
            f => new CustomerRequest(f.Name.FullName(), f.Internet.Email())
        );

        return customerFaker.Generate(count);
    }

    // Generate customer with domain-specific email
    public static CustomerRequest RandomCustomerWithDomain(string domain)
    {
        return new CustomerRequest(
            Faker.Name.FullName(),
            Faker.Internet.Email(provider: domain)
        );
    }
}
