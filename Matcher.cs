using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Renamer
{
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
            newP = "clipboard.jpg";
            res = "clipboard.jpg"
            test(files1, oldP, newP, res);

            oldP = "clipboard.jpg";
            newP = "aaa-clipboard01.jpg";        
            res = "aaa-clipboard01.jpg"
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


        //public static void Main(string[] args)
        //{
        //    int RUN_DEBUG = 1;

        //    if (RUN_DEBUG == 1)
        //    {
        //        runTests();
        //        Console.ReadKey();
        //        return;
        //    }


        //    //work on matcher...
            

        //    Console.WriteLine("ToDo: current work on matcher...");
        //    Console.ReadKey();
        //}


    }
}
