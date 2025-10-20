using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            if (Regex.IsMatch(oldPattern, @"^\d{8}\*\.[a-zA-Z0-9]+$") &&
                Regex.IsMatch(newPattern, @"^\d{2}-\d{2}-\d{4}\*\.[a-zA-Z0-9]+$"))
            {
                Regex dateRegex = new Regex(@"^(?<date>\d{8})(?<rest>.*)\.(?<extension>[a-zA-Z0-9]+)$", RegexOptions.IgnoreCase);
                foreach (string oldFilePath in files)
                {
                    string filename = Path.GetFileName(oldFilePath);
                    Match m = dateRegex.Match(filename);
                    if (m.Success)
                    {
                        string date = m.Groups["date"].Value;
                        string rest = m.Groups["rest"].Value;
                        string extension = m.Groups["extension"].Value;
                        string formattedDate = $"{date.Substring(0, 2)}-{date.Substring(2, 2)}-{date.Substring(4, 4)}";
                        string newFileName = $"{formattedDate}{rest}.{extension}";
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
            else
            {
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

    static class Matcher
    {
        static string VERSION = "V1.0";

        public static List<string> match(string oldName, string newName, List<string> files)
        {
            // Escape regex special chars, except * and ?
            string regexPattern = "^" + Regex.Escape(oldName)
                .Replace(@"\*", "(.*)")   // capture *
                .Replace(@"\?", "(.)")    // capture ?
                + "$";

            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            List<string> results = new List<string>();

            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                Match match = regex.Match(filename);

                if (!match.Success)
                {
                    results.Add(filename); // keep original if not matching
                    continue;
                }

                // Replace wildcards in newName with captured groups
                string newFilename = newName;
                int groupIndex = 1;

                // handle * and ? in newName
                foreach (char c in newName.ToCharArray())
                {
                    if (c == '*' || c == '?')
                    {
                        newFilename = newFilename.Replace(c.ToString(), match.Groups[groupIndex].Value, 1);
                        groupIndex++;
                    }
                }

                results.Add(newFilename);
            }

            return results;
        }

        public static string Replace(this string text, string search, string replace, int count)
        {
            int index = text.IndexOf(search);
            if (index < 0) return text;
            return text.Substring(0, index) + replace + text.Substring(index + search.Length);
        }

        static void runTests()
        {
            Console.WriteLine("Run All Matcher Tests");
            string oldP = "", newP = "", res = "";
            string[] files1 = {"clipboard01.jpg", "clipboard02.jpg", "clipboard03.jpg",
                               "clipboard01.gif", "img01.jpg", "img-abc.jpg" };

            oldP = "clipboard01.jpg";
            newP = "clipboard01.jpg";
            res = "clipboard01.jpg clipboard02.jpg clipboard03.jpg clipboard01.gif img01.jpg img-abc.jpg";
            test(files1, oldP, newP, res);

            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("All tests succeeded!");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey();
        }

        private static void test(string[] files, string oldName, string newName, string? testRes = null)
        {
            Console.WriteLine($"oldName:{oldName} newName: {newName}");
            List<string> res = match(oldName, newName, new List<string>(files));
            string resS = string.Join(" ", res);
            Console.WriteLine("Old:" + string.Join(" ", new List<string>(files)));
            Console.WriteLine("New:" + resS);
            Console.WriteLine("--------------------------------------------------");
            if (testRes != null && resS != testRes)
            {
                throw new Exception("Test failed: expected:" + testRes + " received:" + resS);
            }
        }
    }

    public static class RegexConverter
    {
        public static Regex convert(string wildcard, bool ignoreCase = true)
        {
            string regexPattern = "^" + Regex.Escape(wildcard)
                                         .Replace(@"\*", ".*")
                                         .Replace(@"\?", ".") + "$";

            return new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public static Regex newPattern(string input, bool isRegex, bool ignoreCase = true)
        {
            return isRegex
                ? new Regex (input, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)
                : convert(input, ignoreCase);
        }
    }
}

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

        public static void RenameFiles(string directory, string wildcard, string replacePattern)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Directory does not exist");
                return;
            }

            var regex = Renamer.RegexConverter.newPattern(wildcard, true, true);

            foreach (var file in matchingFiles(directory, regex))
            {
                string currentName = Path.GetFileName(file);
                string newName = regex.Replace(currentName, replacePattern);
                string newPath = Path.Combine(directory, newName);

                if (!File.Exists(newPath))
                {
                    File.Move(file, newPath);
                    Console.WriteLine($"Renamed: {currentName} -> {newName}");
                }
                else
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