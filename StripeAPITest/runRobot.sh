#!/bin/bash

echo "🧹 Formating code..."

dotnet csharpier format .

echo "🧪 Running tests with Allure..."

# Clean old results
rm -rf bin/allure-results bin/allure-report

# Run tests
dotnet test

# Check if allure-results directory exists and has files
if [ -d "bin/allure-results" ] && [ "$(ls -A bin/allure-results)" ]; then
    echo ""
    echo "📊 Generating Allure report..."
    
    # Generate and serve report
    npx allure serve bin/allure-results
else
    echo "❌ No test results found in allure-results directory"
    echo "Make sure tests ran successfully and [AllureNUnit] attribute is applied"
fi
