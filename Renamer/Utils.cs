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

            var regex = RegexConverter.newPattern(wildcard, true, true); // testweise aufgrund von Fehlermeldung

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

        public static string ConvertDateInFilename(string filename)
        {
            // Match 5 or 6 digits in a row
            var match = Regex.Match(filename, @"\b\d{5,6}\b");
            if (!match.Success) return filename; // no date → return original

            string digits = match.Value;

            // Normalize to 6 digits (pad left with 0 if 5-digit)
            if (digits.Length == 5)
                digits = digits.PadLeft(6, '0'); // e.g. 31225 → 031225

            // Split into dd, MM, yy
            string day = digits.Substring(0, 2);
            string month = digits.Substring(2, 2);
            string year = digits.Substring(4, 2);

            string formatted = $"{day}-{month}-{year}";

            // Replace the raw digits with formatted date
            return filename.Replace(match.Value, formatted);
        }



    }
}
