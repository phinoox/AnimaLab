using System;
using System.IO;
using System.Runtime.InteropServices;
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

NamespaceFixer.Run(rootDirectory);

public static class NamespaceFixer
{
    private static string _nsPattern = @"(namespace.*;)";
    private static string _projectsLocation = "src/Anima";

    public static void Run(string rootPath)
    {
        if (!Directory.Exists(rootPath))
        {
            Console.WriteLine("could not find directory)");
            return;
        }
        // Get all files in the directory and subdirectories
        var allFiles = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in allFiles)
        {
            if (!File.Exists(file)) continue;

            string? folderName = Path.GetDirectoryName(file);
            int location = folderName!.IndexOf(_projectsLocation);
            if (location < 0)
            {
                Console.WriteLine($"Error: could not get filepath relativ to project src folder for {file}");
                continue;
            }

            string relativeLocation = folderName.Substring(location + 4); // looking for complete src/gadema, but removing only src/

            string namespaceName = relativeLocation.Replace(Path.DirectorySeparatorChar, '.');
            string newNameSpace = $"namespace {namespaceName};";



            string content = File.ReadAllText(file);
            Regex regex = new Regex(_nsPattern);

            Match match = regex.Match(content);
            if (newNameSpace.Equals(match.Value)){
                Console.WriteLine($"namespace for {file} was already set correctly");
                continue;
            }


            if (match.Success)
            {
                string newContent = content.Replace(match.Value, newNameSpace);
                try
                {
                    File.WriteAllText(file, newContent);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: could not save {file}. message was {e.Message}");
                    continue;
                }
            }
            else
            {
                Console.WriteLine($"Error: could not find namespace in {file}");
                continue;
            }


            Console.WriteLine($" namespace has been renamed from {match.Value} to {newNameSpace}");


        }
    }
}