
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

Printer.PrintFks();


public class Printer
{
    public static void PrintFks()
    {
        string rootPath = @"src/Gadema.Core/Models";
        string outputPath = "scripts/output/modelfks.md";
        string pattern = @"(.*public class.*)|(.*Guid.*)|(.*ForeignKey.*\n.*)|(.*ICollection.*)|(.*namespace.*)";
        string output = "";
        StringBuilder sb = new StringBuilder();
        // SearchOption.AllDirectories ensures recursion into all subfolders
        string[] files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);
        
        foreach (string file in files)
        {
            
            string fileContents = File.ReadAllText(file);
            MatchCollection matches = Regex.Matches(fileContents, pattern, RegexOptions.IgnoreCase);
            if(matches.Count == 0)
                continue;
            sb.AppendLine(file);
            foreach (Match match in matches)
            {
                sb.AppendLine(match.Value);
            }
            sb.AppendLine();
        }
        output = sb.ToString();
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.Write(output);
        }
    }
}
