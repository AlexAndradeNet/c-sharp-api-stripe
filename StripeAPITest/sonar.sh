TOKEN=$(echo $SONAR_TOKEN)

if [ -z "$TOKEN" ]; then
  echo "SONAR_TOKEN system variable is not set. Exiting."
  exit 1
fi

dotnet csharpier format .

dotnet sonarscanner begin /k:"com-stripe-api" \
    /d:sonar.host.url="http://localhost:9000" \
    /d:sonar.token="$TOKEN" \
    /d:sonar.exclusions="**/bin/**, **/obj/**, **/.sonarqube/**, **/.sonar/**, **/node_modules/**" \
    /d:sonar.sources="Src/Main" \
    /d:sonar.tests="Src/Tests" \
    /d:sonar.typescript.exclusions="**/bin/**" \
    /d:sonar.javascript.exclusions="**/bin/**" \
    /d:sonar.sourceEncoding='UTF-8' \
    /d:sonar.coverage.exclusions="**/*.cs"

dotnet build

dotnet sonarscanner end /d:sonar.token="$TOKEN" 
