namespace StripeAPITest.Main.Utils;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

public abstract class SchemaValidator
{
    private SchemaValidator()
    {
        // Empty private constructor to prevent instantiation
    }

    /// <summary>
    /// Validates a JSON data string against a JSON schema string.
    /// </summary>
    /// <param name="schemaJson">The JSON schema content as a string.</param>
    /// <param name="dataJson">The JSON data content as a string.</param>
    /// <returns>True if the data is valid against the schema; otherwise, false.</returns>
    public static bool Validate(string schemaJson, string dataJson)
    {
        // 1. Parse the schema string into a JSchema object
        JSchema schema = JSchema.Parse(schemaJson);

        // 2. Parse the data string into a JToken object
        JObject data = JObject.Parse(dataJson);

        // 3. Perform the validation
        bool isValid = data.IsValid(schema, out IList<string> errorMessages);
        if (!isValid)
        {
            // Concatenate the errors into a single, detailed message
            string errorMessage =
                "JSON failed schema validation:\n"
                + string.Join("\n", errorMessages);

            // Throw the specific validation exception
            throw new JSchemaValidationException(errorMessage);
        }
        return isValid;
    }

    public static bool ValidateFromFiles(string schemaFilePath, string dataJson)
    {
        // Load schema and data from files
        string schemaJson = FileLoader.ReadFileContent(
            "Tests/Resources/Schemas/" + schemaFilePath
        );

        // Validate the data against the schema
        try
        {
            return Validate(schemaJson, dataJson);
        }
        catch (JSchemaValidationException e)
        {
            string detailedMessage =
                $"Validation failed for schema file '{schemaFilePath}': {e.Message}";
            throw new JSchemaValidationException(detailedMessage);
        }
    }
}
