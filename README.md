# CSharp project in VSCode

This is in VSCode, not Visual Studio Code; the blue one not the purple IDE.

## Conventions

- Csharp is also called C# or DotNet or .Net.
- The language uses K&R coding conventions. It's noticable the use of brakets in a mode different than eghyptian.
- NuGet is the dependencies manager.

## Installation

### Install DotNet

brew install dotnet

### Start the project

dotnet new sln -n StripeAPITest
mkdir StripeAPITest
cd StripeAPITest
dotnet new nunit
cd ..
dotnet sln add StripeAPITest
dotnet new gitignore

### Install the IDE tools

Install the following extensions:
- C# Dev Kit, Official extension from Microsoft
- .NET Core Test Explorer
- NuGet Package Manager
- vscode-solution-explorer

Then, reopen the project.

### Initial file modification

Change the contents of Program.cs to:

\\csharp
class Program
{
    static void Main()
    {
        Console.WriteLine("Hellow C# world");
    }
}

## Run for the first time

Even there is no code, it's a good idea to run the project to create other support files.

cd StripeAPITest
dotnet run

## Install NuGet Dependiencies

dotnet add package Microsoft.Playwright

## Development


### Reporting

dotnet tool install trx2html

### Linting

donet tool install csharpier

dotnet csharpier format .
