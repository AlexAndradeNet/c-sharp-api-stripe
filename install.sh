#!/bin/bash

npm install

cd StripeAPITest
dotnet restore
dotnet tool install csharpier
dotnet tool install dotnet-sonarscanner
