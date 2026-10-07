using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run scripts/filediff.cs <file1> <file2>");
    return;
}

string file1 = args[0];
string file2 = args[1];
FileDiffer.Diff(file1, file2);

enum DiffType { Added, Removed, Unchanged }

// Line1: line number in file 1 (if exists)
// Line2: line number in file 2 (if exists)
record DiffLine(DiffType Type, int Line1, int Line2, string Text);

record Block(
    int StartLine1, 
    int EndLine1, 
    int StartLine2, 
    int EndLine2, 
    List<DiffLine> Lines
);

public class FileDiffer
{
    public static void Diff(string file1, string file2)
    {
        if (!File.Exists(file1))
        {
            Console.WriteLine($"Error: File not found: {file1}");
            return;
        }

        if (!File.Exists(file2))
        {
            Console.WriteLine($"Error: File not found: {file2}");
            return;
        }

        string[] lines1 = File.ReadAllLines(file1);
        string[] lines2 = File.ReadAllLines(file2);

        // 1. Compute the full diff including Unchanged lines (with dual line numbers)
        var allLines = ComputeDiff(lines1, lines2);

        // 2. Group changes into blocks using the new Block record
        var blocks = GroupIntoBlocks(allLines, contextSize: 3);

        Console.WriteLine($"FILE_DIFF: {file1} -> {file2}");
        if (blocks.Count == 0)
        {
            Console.WriteLine("No changes detected.");
            return;
        }

        foreach (var block in blocks)
        {
            // Print standard Unified Diff header: @@ -start,len +start,len @@
            int len1 = block.EndLine1 - block.StartLine1 + 1;
            int len2 = block.EndLine2 - block.StartLine2 + 1;
            
            // If a file has no lines in this block (e.g. pure addition), length is technically 0 for that side
            // but we use the range logic to keep it simple and compatible with standard diff viewers
            Console.WriteLine($"\n@@ -{block.StartLine1},{Math.Max(0, len1)} +{block.StartLine2},{Math.Max(0, len2)} @@");

            foreach (var line in block.Lines)
            {
                string prefix = line.Type switch
                {
                    DiffType.Added => "+ ",
                    DiffType.Removed => "- ",
                    _ => "  " 
                };
                Console.WriteLine($"{prefix}{line.Text}");
            }
        }
    }

    private static List<Block> GroupIntoBlocks(List<DiffLine> allLines, int contextSize)
    {
        int n = allLines.Count;
        if (n == 0) return new List<Block>();

        // Identify indices of lines that are actual changes
        var changeIndices = new List<int>();
        for (int i = 0; i < n; i++)
        {
            if (allLines[i].Type != DiffType.Unchanged)
                changeIndices.Add(i);
        }

        if (changeIndices.Count == 0) return new List<Block>();

        // Create expanded ranges around each change to include context
        var ranges = new List<(int start, int end)>();
        foreach (var idx in changeIndices)
        {
            ranges.Add((Math.Max(0, idx - contextSize), Math.Min(n - 1, idx + contextSize)));
        }

        // Merge overlapping/adjacent ranges to create contiguous blocks
        var mergedRanges = new List<(int start, int end)>();
        var current = ranges[0];
        for (int i = 1; i < ranges.Count; i++)
        {
            if (ranges[i].start <= current.end + 1)
                current = (current.start, Math.Max(current.end, ranges[i].end));
            else
            {
                mergedRanges.Add(current);
                current = ranges[i];
            }
        }
        mergedRanges.Add(current);

        // Convert merged ranges into Block records with correct file line numbers
        var blocks = new List<Block>();
        foreach (var range in mergedRanges)
        {
            var blockLines = new List<DiffLine>();
            int minL1 = int.MaxValue, maxL1 = 0;
            int minL2 = int.MaxValue, maxL2 = 0;

            for (int i = range.start; i <= range.end; i++)
            {
                var line = allLines[i];
                blockLines.Add(line);

                // Track boundaries for File 1
                if (line.Line1 > 0) {
                    minL1 = Math.Min(minL1, line.Line1);
                    maxL1 = Math.Max(maxL1, line.Line1);
                }
                // Track boundaries for File 2
                if (line.Line2 > 0) {
                    minL2 = Math.Min(minL2, line.Line2);
                    maxL2 = Math.Max(maxL2, line.Line2);
                }
            }

            blocks.Add(new Block(
                StartLine1: minL1 == int.MaxValue ? 0 : minL1,
                EndLine1: maxL1 == 0 ? 0 : maxL1,
                StartLine2: minL2 == int.MaxValue ? 0 : minL2,
                EndLine2: maxL2 == 0 ? 0 : maxL2,
                Lines: blockLines
            ));
        }

        return blocks;
    }

    static List<DiffLine> ComputeDiff(string[] s1, string[] s2)
    {
        int[,] table = ComputeLcsTable(s1, s2);
        return BacktrackLcs(table, s1, s2);
    }

    static int[,] ComputeLcsTable(string[] s1, string[] s2)
    {
        int m = s1.Length;
        int n = s2.Length;
        int[,] table = new int[m + 1, n + 1];
        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (s1[i - 1] == s2[j - 1]) table[i, j] = table[i - 1, j - 1] + 1;
                else table[i, j] = Math.Max(table[i - 1, j], table[i, j - 1]);
            }
        }
        return table;
    }

    static List<DiffLine> BacktrackLcs(int[,] table, string[] s1, string[] s2)
    {
        var result = new List<DiffLine>();
        int i = s1.Length;
        int j = s2.Length;

        while (i > 0 || j > 0)
        {
            if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1])
            {
                // Unchanged: Line exists in both files
                result.Add(new DiffLine(DiffType.Unchanged, i, j, s1[i - 1]));
                i--; j--;
            }
            else if (j > 0 && (i == 0 || table[i, j - 1] >= table[i - 1, j]))
            {
                // Added: Line exists only in File 2
                result.Add(new DiffLine(DiffType.Added, 0, j, s2[j - 1]));
                j--;
            }
            else if (i > 0 && (j == 0 || table[i, j - 1] < table[i - 1, j]))
            {
                // Removed: Line exists only in File 1
                result.Add(new DiffLine(DiffType.Removed, i, 0, s1[i - 1]));
                i--;
            }
        }
        result.Reverse();
        return result;
    }
}