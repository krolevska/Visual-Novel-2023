using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    /// <summary>
    /// Reads a text file from disk and returns its content as a list of strings.
    /// </summary>
    /// <param name="filePath">Path to the text file.</param>
    /// <param name="includeBlankLines">Flag to include or exclude blank lines.</param>
    /// <returns>List of strings representing the content of the text file.</returns>
    public static List<string> ReadTextFile(string filePath, bool includeBlankLines = true)
    {
        // Ensure the file path starts with a root slash
        if (!filePath.StartsWith('/'))
            filePath = FilePaths.root + filePath;

        // List to hold the lines read from the file
        List<string> lines = new List<string>();

        try
        {
            // Open a stream reader to read the text file
            using (StreamReader sr = new StreamReader(filePath))
            {
                // Read until the end of the stream
                while (!sr.EndOfStream)
                {
                    // Read a line from the text file
                    string line = sr.ReadLine();

                    // Add the line to the list if it's not blank or if blank lines are included
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
        }
        catch (FileNotFoundException ex)  // Catch file not found exception
        {
            // Log an error message
            Debug.LogError($"File not found: '{ex.FileName}");
        }

        // Return the list of lines
        return lines;
    }

    /// <summary>
    /// Reads a TextAsset and returns its content as a list of strings.
    /// </summary>
    /// <param name="filePath">Path to the TextAsset.</param>
    /// <param name="includeBlankLines">Flag to include or exclude blank lines.</param>
    /// <returns>List of strings representing the content of the TextAsset.</returns>
    public static List<string> ReadTextAsset(string filePath, bool includeBlankLines = true)
    {
        TextAsset asset = Resources.Load<TextAsset>(filePath);
        if (asset == null)
        {
            Debug.LogError($"Asset not found: '{filePath}'");
            return null;
        }
        return ReadTextAsset(asset, includeBlankLines);
    }

    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(asset.text))
        {
            while (sr.Peek() > -1)
            {                    
                // Read a line from the text file
                string line = sr.ReadLine();

                // Add the line to the list if it's not blank or if blank lines are included
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }
        }
        return lines;
    }
}
