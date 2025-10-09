using System;
using System.IO;
using System.Collections.Generic;
using Renamer;

namespace Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Renamer.exe <oldPattern> <newPattern> [occurrence]");
                return;
            }

            string oldPattern = args[0];
            string newPattern = args[1];
            int occurrence = -1;
            if (args.Length >= 3 && int.TryParse(args[2], out int n) && n > 0)
            {
                occurrence = n;
            }

            string currentDir = Directory.GetCurrentDirectory();
            List<string> files = new List<string>(Directory.GetFiles(currentDir));

            for (int i = 0; i < files.Count; i++)
            {
                string oldFilePath = files[i];
                string filename = Path.GetFileName(oldFilePath);
                string newFileName;

                // If the oldPattern is found as a substring, replace it directly
                if (filename.Contains(oldPattern) && !oldPattern.Contains("*") && !oldPattern.Contains("?"))
                {
                    if (occurrence > 0)
                    {
                        // Use the extension method from Matcher.cs
                        newFileName = Matcher.Replace(filename, oldPattern, newPattern, occurrence);
                    }
                    else
                    {
                        newFileName = filename.Replace(oldPattern, newPattern);
                    }
                }
                else
                {
                    // Use Matcher.match for wildcard patterns
                    List<string> singleResult = Matcher.match(oldPattern, newPattern, new List<string> { oldFilePath });
                    newFileName = singleResult[0];
                }

                // Only rename if the name has changed
                if (filename != newFileName)
                {
                    string newFilePath = Path.Combine(currentDir, newFileName);
                    if (!File.Exists(newFilePath))
                    {
                        File.Move(oldFilePath, newFilePath);
                        Console.WriteLine($"Renamed: {oldFilePath} -> {newFilePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Skipped (target exists): {newFilePath}");
                    }
                }
            }
        }
    }
}
