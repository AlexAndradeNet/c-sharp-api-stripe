namespace StripeAPITest.Main.Utils;

using System.IO;

public static class FileLoader
{
    /// <summary>
    /// Reads the entire content of a file into a string variable using StreamReader.
    /// </summary>
    /// <param name="filePath">The path to the file (schema or data).</param>
    /// <returns>The file content as a string.</returns>
    public static string ReadFileContent(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"The file was not found at path: {filePath}"
            );
        }

        // Use StreamReader within a 'using' block for efficient and safe resource management
        using StreamReader file = File.OpenText(@"" + filePath);
        return file.ReadToEnd();
    }
}
