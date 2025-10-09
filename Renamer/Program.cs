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

            if (Regex.IsMatch(oldPattern, @"^\d{8}\*\.jpg$") && Regex.IsMatch(newPattern, @"^\d{2}-\d{2}-\d{4}\*\.jpg$"))
            {
                Regex dateRegex = new Regex(@"^(?<date>\d{8})(?<rest>.*)\.jpg$", RegexOptions.IgnoreCase);
                foreach (string oldFilePath in files)
                {
                    string filename = Path.GetFileName(oldFilePath);
                    Match m = dateRegex.Match(filename);
                    if (m.Success)
                    {
                        string date = m.Groups["date"].Value;
                        string rest = m.Groups["rest"].Value;
                        string formattedDate = $"{date.Substring(0, 2)}-{date.Substring(2, 2)}-{date.Substring(4, 4)}";
                        string newFileName = $"{formattedDate}{rest}.jpg";
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

                // If newPattern contains a number placeholder (e.g., 001), use numbering
                if (newPattern.Contains("001"))
            {
                int count = 1;
                int digits = 3; // Default to 3 digits, can be inferred from newPattern
                int digitStart = newPattern.IndexOf("001");
                while (digitStart > 0 && char.IsDigit(newPattern[digitStart - 1]))
                {
                    digitStart--;
                    digits++;
                }

                foreach (string oldFilePath in files)
                {
                    string filename = Path.GetFileName(oldFilePath);
                    // Use Matcher.match to check if file matches the pattern
                    List<string> matchResult = Matcher.match(oldPattern, newPattern, new List<string> { oldFilePath });
                    string matchedName = matchResult[0];

                    // Only rename if matched
                    if (matchedName != filename)
                    {
                        // Format number with leading zeros
                        string number = count.ToString().PadLeft(digits, '0');
                        string newFileName = newPattern.Replace("001", number);

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
                        count++;
                    }
                }
            }
            else
            {
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
                            newFileName = Matcher.Replace(filename, oldPattern, newPattern, occurrence);
                        }
                        else
                        {
                            newFileName = filename.Replace(oldPattern, newPattern);
                        }
                    }
                    else
                    {
                        List<string> singleResult = Matcher.match(oldPattern, newPattern, new List<string> { oldFilePath });
                        newFileName = singleResult[0];
                    }

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
}
