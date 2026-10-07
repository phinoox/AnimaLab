using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run scripts/rotate_files.cs <directory_path>");
    return;
}

string rootDirectory = args[0];

if (!Directory.Exists(rootDirectory))
{
    Console.WriteLine($"Error: Directory not found: {rootDirectory}");
    return;
}

ModelPrinter.PrintSkeleton(rootDirectory);

public static class ModelPrinter
{
    static string pattern = "\\s*(public.*|private.*|protected.*)";
    public static void PrintSkeleton(string rootPath)
    {
        // Get all files in the directory and subdirectories
        var allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

        foreach (var filePath in allFiles)
        {
            string directory = Path.GetDirectoryName(filePath)!;
            string fileName = Path.GetFileName(filePath);

            // We only want to process files that ARE NOT already .bak and ARE NOT prefixed with new_
            // This prevents infinite loops or double processing
            if (!fileName.EndsWith(".cs") )
            {
                continue;
            }

            

            // Check if the 'new_' version exists
            if (File.Exists(filePath))
            {
                StringBuilder sb = new StringBuilder();
                try
                {
                    var content = File.ReadAllText(filePath);
                    MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnorePatternWhitespace);
                    if (matches.Count == 0)
                        continue;
                    sb.AppendLine($"==> {filePath} <==");
                    foreach (Match match in matches)
                    {
                        sb.AppendLine(match.Value);
                    }
                    sb.AppendLine();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {fileName}: {ex.Message}");
                }
                
                Console.WriteLine(sb.ToString());
            }
        }
    }
}