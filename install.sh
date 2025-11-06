#!/bin/bash

npm install

dotnet tool install csharpier
dotnet tool install dotnet-sonarscanner

dotnet tool install Husky
dotnet husky install

chmod +x .husky/pre-commit
chmod +x .husky/pre-push

cd StripeAPITest
dotnet restore
