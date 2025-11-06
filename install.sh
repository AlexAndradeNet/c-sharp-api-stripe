#!/bin/bash

npm install

dotnet tool install csharpier
dotnet tool install dotnet-sonarscanner

chmod +x .husky/pre-commit
chmod +x .husky/pre-push
dotnet tool install Husky
dotnet husky install

cd StripeAPITest
dotnet restore
