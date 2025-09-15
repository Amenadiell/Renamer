using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Utils
{
    public class Files
    {

        // List the Files matching input pattern
        static List<string> matchingFiles(string path, Regex pattern)
        {
            var files = Directory.GetFiles(path);
            List<string> matchedFiles = new List<string>(files.Length);
            foreach (var file in files)
            {
                var match = pattern.Match(file);
                if (match.Success) matchedFiles.Add(match.Groups[1].Value);
            }
            return matchedFiles;
        }


        public static void RenameFiles(string directory,string wildcard,string replacePattern)
        {
            if (!Directory.Exists(directory)) 
            {
                Console.WriteLine("Directory does not exist");
                return;
            }

            var regex = RegexConverter.newPattern(wildcard);

            foreach (var file in matchingFiles(directory,regex))
            {
                string currentName = Path.GetFileName(file);
                string newName = regex.Replace(currentName, replacePattern);
                string newPath = Path.Combine(directory,newName);

                if (!File.Exists(newPath))
                {
                    File.Move(file,newPath);
                    Console.WriteLine($"Renamed: {currentName} -> {newName}");
                }else
                {
                    Console.WriteLine($"Skipped {currentName} (already exists)");
                }
            }
        }

    }
}
