// <copyright file="CustomerRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace StripeAPITest.Main.Models;

public class CustomerRequest(string name, string email)
{
    public string Name { get; } = name;

    public string Email { get; } = email;
}
