using System;
using System.IO;

namespace ReadMeDaddy
{
    public static class FileOperations
    {
        // Method to read text from a .txt file only.
        public static string ReadTextFromFile(string filePath)
        {
            try
            {
                if (Path.GetExtension(filePath).ToLower() == ".txt")
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    return "Unsupported file format. Only .txt files are supported.";
                }
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }

        // Method to append text to a .txt file only.
        public static void AppendTextToFile(string filePath, string content)
        {
            try
            {
                if (Path.GetExtension(filePath).ToLower() == ".txt")
                {
                    File.AppendAllText(filePath, "\n" + content);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported file format. Only .txt files can be updated.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to append text to file: {ex.Message}");
            }
        }
    }
}
