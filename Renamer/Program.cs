using System;
using System.IO;
using System.Collections.Generic;
using Renamer;   // <-- this makes Matcher visible

namespace Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: Renamer <directory> <oldPattern> <newPattern>");
                Console.WriteLine("Example: Renamer C:\\files img-*.jpg *-img.jpg");
                return;
            }

            string directory = args[0];
            string oldPattern = args[1];
            string newPattern = args[2];

            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Directory not found: {directory}");
                return;
            }

            var files = Directory.GetFiles(directory);
            var newNames = Matcher.matcher(oldPattern, newPattern, new List<string>(files));

            for (int i = 0; i < files.Length; i++)
            {
                string oldPath = files[i];
                string oldName = Path.GetFileName(oldPath);
                string newName = newNames[i];

                if (oldName == newName) continue; // no change

                string newPath = Path.Combine(directory, newName);

                if (File.Exists(newPath))
                {
                    Console.WriteLine($"Skipped {oldName} (target {newName} already exists)");
                    continue;
                }

                File.Move(oldPath, newPath);
                Console.WriteLine($"Renamed: {oldName} -> {newName}");
            }
        }
    }
}
