using System;
using System.IO;

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

FileRotator.Rotate(rootDirectory);

public static class FileRotator
{
    public static void Rotate(string rootPath)
    {
        // Get all files in the directory and subdirectories
        var allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

        foreach (var filePath in allFiles)
        {
            string directory = Path.GetDirectoryName(filePath)!;
            string fileName = Path.GetFileName(filePath);

            // We only want to process files that ARE NOT already .bak and ARE NOT prefixed with new_
            // This prevents infinite loops or double processing
            if (fileName.EndsWith(".bak") || fileName.StartsWith("new_"))
            {
                continue;
            }

            string newFilePath = Path.Combine(directory, "new_" + fileName);

            // Check if the 'new_' version exists
            if (File.Exists(newFilePath))
            {
                try
                {
                    string bakPath = filePath + ".bak";

                    // 1. Rename [file] to [file].bak
                    // If .bak already exists, we delete it first to allow rename
                    if (File.Exists(bakPath))
                    {
                        File.Delete(bakPath);
                    }
                    
                    File.Move(filePath, bakPath);
                    Console.WriteLine($"Renamed: {fileName} -> {Path.GetFileName(bakPath)}");

                    // 2. Rename new_[file] to [file]
                    File.Move(newFilePath, filePath);
                    Console.WriteLine($"Updated: {newFilePath} -> {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {fileName}: {ex.Message}");
                }
            }
        }
    }
}