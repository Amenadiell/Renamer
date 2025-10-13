# Ki Dokumentation – FileRenamer  

## Generelle Beschreibung des Ablaufes:  

Die KI, die wir zum Ausarbeiten des Projektes genutzt haben, ist ChatGPT.  

Um uns einen generellen Überblick und einen ersten Startpunkt von der KI zu erhalten, haben wir die Aufgabenstellung im Ganzen kopiert und der KI als ersten Promt gegeben. Im weiteren Verlauf der Projektarbeit haben wir dann diesen Code, der natürlich noch nicht funktionsfähig war von der KI verbessern lassen und neue Ideen miteinfließen lassen. Im letzten Schritt haben wir dann mithilfe der KI kontrolliert, ob der Code, welchen wir geschrieben haben, so funktionieren kann. 

# KI Promts und Antworten  

### Startpromt

Entwickle ein Tool in C#, das in der Lage ist, Dateinamen-Muster zu erkennen und anschließend alle passenden Dateien in einem angegebenen Verzeichnis (inkl. Unterordner) automatisch umzubenennen. Besonderes Augenmerk liegt dabei auf der intelligenten Behandlung von Zahlenfolgen, etwa durch Einfügen führender Nullen zur Vereinheitlichung. 
Beispieldateiname als Vorlage: 
 Der Benutzer gibt ein Beispiel für das gewünschte Dateinamen-Muster an (z. B. Bild_001.jpg), anhand dessen das Programm die allgemeine Struktur ableitet. 
Mustererkennung: 
 Das Programm analysiert vorhandene Dateinamen und erkennt variable Komponenten – insbesondere Zahlen –, um ein konsistentes Muster zu ermitteln. 
Automatische Umbenennung: 
 Auf Basis des erkannten Musters werden alle passenden Dateien umbenannt. Zahlen können dabei automatisch mit führenden Nullen ergänzt oder formatiert werden (z. B. 1 → 001). 
Verzeichnis-Rekursion: 
 Alle Dateien in einem gewählten Verzeichnis inklusive Unterordner werden berücksichtigt. 
 
<details>
  <summary> Code ansehen</summary>

```csharp
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FilePatternRenamer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Automatisches Datei-Umbenennungs-Tool ===");
            Console.Write("Pfad zum Startverzeichnis: ");
            string basePath = Console.ReadLine()?.Trim('"');

            if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
            {
                Console.WriteLine("❌ Ungültiges Verzeichnis.");
                return;
            }

            Console.Write("Beispiel-Dateiname (z. B. Bild_001.jpg): ");
            string sample = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(sample) || !sample.Contains('.'))
            {
                Console.WriteLine("❌ Ungültiger Beispielname.");
                return;
            }

            // Musteranalyse
            var pattern = BuildPatternFromSample(sample, out string numberFormat);
            Console.WriteLine($"\nErkanntes Muster: {pattern}");
            Console.WriteLine($"Nummernformat: {numberFormat}\n");

            // Alle Dateien finden
            var allFiles = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);
            var matchingFiles = allFiles.Where(f => Regex.IsMatch(Path.GetFileName(f), pattern)).ToList();

            if (matchingFiles.Count == 0)
            {
                Console.WriteLine("Keine passenden Dateien gefunden.");
                return;
            }

            Console.WriteLine($"Gefundene Dateien: {matchingFiles.Count}");
            Console.WriteLine("Dateien werden umbenannt...\n");

            foreach (var file in matchingFiles)
            {
                string fileName = Path.GetFileName(file);
                var match = Regex.Match(fileName, pattern);
                if (!match.Success) continue;

                string number = match.Groups["num"].Value;
                if (int.TryParse(number, out int numValue))
                {
                    string newNumber = numValue.ToString(numberFormat);
                    string newFileName = Regex.Replace(fileName, number, newNumber);
                    string newFullPath = Path.Combine(Path.GetDirectoryName(file), newFileName);

                    if (!File.Exists(newFullPath))
                    {
                        File.Move(file, newFullPath);
                        Console.WriteLine($"✔ {fileName} → {newFileName}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠ Übersprungen (existiert bereits): {newFileName}");
                    }
                }
            }

            Console.WriteLine("\nFertig! Alle passenden Dateien wurden vereinheitlicht umbenannt.");
        }

        /// <summary>
        /// Erkennt anhand des Beispiels ein Regex-Muster und bestimmt die Formatierung der Zahlen.
        /// </summary>
        static string BuildPatternFromSample(string sample, out string numberFormat)
        {
            var escaped = Regex.Escape(sample);
            var numMatch = Regex.Match(sample, @"(\d+)");
            if (!numMatch.Success)
            {
                numberFormat = "0";
                return "^" + escaped + "$";
            }

            string numPart = numMatch.Value;
            numberFormat = new string('0', numPart.Length);

            // Beispiel: Bild_001.jpg → ^Bild_(?<num>\d+)\.jpg$
            string pattern = "^" + Regex.Replace(escaped, Regex.Escape(numPart), @"(?<num>\d+)") + "$";
            return pattern;
        }
    }
}

```
</details>

### Weitere Ausarbeitung des Startcodes
Als nächstest wollten wir von der KI ein funtionirendes Regex Pattern um unsere Files zu verändern

#### Antwort der KI: 
Got it 👍 — you want a Regex pattern generator that can take file wildcard expressions (like *.txt or file_??.jpg) and translate them into valid C# Regex patterns that you can use to match filenames.
Here’s the key idea:
* → matches any number of characters → .* in regex
? → matches a single character → . in regex
Literal characters (. \ + etc.) must be escaped so they don’t act as regex operators

Example Conversion
Wildcard	Regex Equivalent	Matches
*.txt	^.*\.txt$	All .txt files
file?.jpg	^file.\.jpg$	file1.jpg, filea.jpg
data*2025.csv	^data.*2025\.csv$	data2025.csv, data_final_2025.csv
report_??.doc	^report_..\.doc$	report_ab.doc, report_12.doc
C# Helper Method

Here’s a utility function to convert wildcard → regex:

<details>
  <summary> Code ansehen</summary>

```csharp
```C#
using System;
using System.Text.RegularExpressions;

public static class WildcardToRegex
{
    public static Regex Convert(string wildcard, bool ignoreCase = true)
    {
        string regexPattern = "^" + Regex.Escape(wildcard)
                                     .Replace(@"\*", ".*")
                                     .Replace(@"\?", ".") + "$";

        return new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
    }
}
```
</details>

Da dies unsere Bedingungen noch nicht zu 100% erfüllte und es noch einige Bugs gab, stellten wir den Promt mit genauen Beispielen, nocheinmal: 

#### Promt: 

I am working on a File Renamer built in C# to match the files to be renamed i need a Regex pattern to match different files with Wildcards
The Renamer should be able to change and delete the Suffix/Präfix with the terminal.

for example : delete the Suffix/Präfix:
Data: img-123.jpg img-3333.jpg img-abc.png
comand: c:\>renamer img-123.jpg 123.jpg
            c:\>renamer img-* *

outcome : 123.jpg 3333.jpg abc.png
comand : c:\>renamer img-123.jpg img-123
                  renamer *.jpg *
outcome: img-123 img-3333 img-abc.png

change the Suffix/Präfix:
comand: c:\>renamer img-123.jpg image-123.jpg 
            c:\>renamer img-* image-*
outcom: image-123.jpg image-3333.jpg image-abc.png
comand: c:\>renamer img-123.jpg img-123.gif
            c:\>renamer *.jpg *.gif
outcome: img-123.gif img-3333.gif img-abc.png

#### Antwort der KI: 

Got it 👍 — thanks for sharing all the code.
Right now, your RegexConverter handles * and ? wildcards, but your matcher() is not actually applying capture groups, which is what you’ll need to map oldName → newName properly.
Here’s the missing piece: you need to generate a Regex with capture groups from the oldName wildcard, and then use those groups to substitute into the newName pattern.
Example

oldName: img-*.jpg
newName: *.gif
File: img-123.jpg
Steps:

Convert oldName → Regex with a group for *:
^img-(.*)\.jpg$
Match img-123.jpg → group(1) = 123
Replace * in newName with group(1):
*.gif → 123.gif

<details>
  <summary> Code ansehen</summary>

```csharp
```C#
public static List<string> matcher(string oldName, string newName, List<string> files)
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
````
</details>

Um der Aufganbenstellung gerecht zu werden, haben wir uns dann noch um die Trennung des Datums mit - gemacht um diese in unseren Code einzubinden. 

#### Antwort der KI: 
Good question 👌 — you basically want a helper that:
Detects whether a date-like string (e.g. 31225) exists in the filename.
Splits it into dd-MM-yy format (→ 31-12-25).
Replaces it in the filename.

</details>

<details>
    <summary> Code anzeigen</summary>

```csharp
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
```
</details>
Diese Implementierung hatte tatsächlich auch seinen Mehrwert, die Trennun funktionierte.

Wir hatten ein Problem den Code auszuführen, da wir mehrere main Methoden hatten, da wir an mehreren Stellen gleichzeitig mit ChatGPT gearbeitet hatten und dieser dann nicht genau mit unserem Code umgehen konnte. 
Dafür haben wir ihm unsere gesammte Projektmappe gegeben um so ChatGPT ein besseren Überblich zu geben, wo wir gerade Probleme haben. 

#### Antwort der KI: 
Perfect 👍 — let’s get your integration fixed first so the project actually runs from the console.
Right now, you have two entry points:
Matcher.Main() in Matcher.cs
Input.Main() in Program.cs (unfinished)
👉 A .NET console app can only have one Main method as the entry point.
Here’s a cleaned-up version of Program.cs that integrates with your utilities and matcher:

</details>

<details>
    <summary> Code anzeigen</summary>

```csharp
using System;
using System.IO;
using Utils;

namespace Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: Renamer <directory> <pattern> <replacement>");
                Console.WriteLine("Example: Renamer C:\\files *.jpg new-*.jpg");
                return;
            }

            string directory = args[0];
            string wildcard = args[1];
            string replacement = args[2];

            try
            {
                Files.RenameFiles(directory, wildcard, replacement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
```
</details>

Als wir dann den Regex-Konverter ausprobieren konnten, fiel uns auf, das dieser nicht so funktionierte, wie gewollt und wir immer eine Fehlermeldung beim Auführen des Programmes bekamen. Deshalb gaben wir ChatGPT den gesammten Code, in dem wir den Converter benutzten und wollten nocheinmal einen Code für den Converter. 

#### Unser Code an die KI: 

</details>

<details>
    <summary> Code anzeigen</summary>

```csharp
My Matcher.cs:
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Renamer
{
    class Matcher
    {
        static string VERSION = "V1.0";
        public static List<string> matcher(string oldName, string newName, List<string> files)
        {

            // Do matching here
            return files;
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

            /*
            oldP = "clipboard01.jpg";
            newP = "aaa-clipboard01.jpg";
            res = "aaa-01.jpg aaa-02.jpg aaa-03.jpg aaa-clipboard01.gif aaa-img01.jpg aaa-img-abc.jpg";
            test(files1, oldP, newP, res);
            */

            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("All tests succeeded!");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey();

        }

        private static void test(string[] files, string oldName, string newName, string testRes = null)
        {
            Console.WriteLine($"oldName:{oldName} newName: {newName}");
            List<string> res = matcher(oldName, newName, new List<string>(files));
            string resS = string.Join(" ", res);
            Console.WriteLine("Old:" + string.Join(" ", new List<string>(files)));
            Console.WriteLine("New:" + resS);
            Console.WriteLine("--------------------------------------------------");
            if (testRes != null && resS != testRes)
            {
                throw new Exception("Test failed: expected:" + testRes + " received:" + resS);
            }
        }


        public static void Main(string[] args)
        {
            int RUN_DEBUG = 1;

            if (RUN_DEBUG == 1)
            {
                runTests();
                Console.ReadKey();
                return;
            }

            //
            // work on matcher...
            //

            Console.WriteLine("ToDo: current work on matcher...");
            Console.ReadKey();
        }


    }
}


Program.cs:
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Renamer
{
    class Input
    {
        static void Main(string[] args)
        {
            string currentDir = Directory.GetCurrentDirectory();
            Matcher.

        }
}


RegexConverter.cs:
using System;
using System.Text.RegularExpressions;

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
            ? new Regex(input, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)
            : convert(input, ignoreCase);
    }



}



Renamer.csproj:

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>


Utils.cs:
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

```
</details>

#### Antwort der KI: 

Got it 👍 You already have most of the scaffolding in place — RegexConverter handles wildcards → regex, and Utils.Files applies renaming.
What’s missing is the matcher logic in Matcher.matcher(): it should take oldName and newName, derive a regex pattern from oldName, and then apply the same transformation to all files in the list.
Here’s a clean way to implement it:

1. Core idea
Convert oldName into a regex with capture groups so you can reuse them in newName.
Example:
oldName = "clipboard01.jpg"
→ Regex: ^(clipboard)(\d+)(\.jpg)$
newName = "aaa-$2.jpg" → output: aaa-01.jpg, aaa-02.jpg, etc.

2. Implementation for Matcher.cs:
</details>

<details>
    <summary> Code anzeigen</summary>

```csharp
namespace Renamer
{
    class Matcher
    {
        static string VERSION = "V1.0";

        public static List<string> matcher(string oldName, string newName, List<string> files)
        {
            // Build regex from oldName (make dynamic groups for digits and text)
            string pattern = Regex.Escape(oldName)
                                  .Replace("01", "(\\d+)")
                                  .Replace("abc", "(.*?)"); // example for flexible text parts

            var regex = new Regex("^" + pattern + "$", RegexOptions.IgnoreCase);

            var renamedFiles = new List<string>(files.Count);
            foreach (var file in files)
            {
                var match = regex.Match(file);
                if (match.Success)
                {
                    // Replace groups into newName
                    string newFile = newName;
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        newFile = newFile.Replace($"${i}", match.Groups[i].Value);
                    }
                    renamedFiles.Add(newFile);
                }
                else
                {
                    // If no match, keep as-is
                    renamedFiles.Add(file);
                }
            }
            return renamedFiles;
        }

        // runTests() stays the same
    }
}
```
</details>

Um dann noch sicher zu gehen ob der Code den uns ChatGPT dort geschrieben hat auch so funktioniert, wie wir es haben wollen, haben wir ihm nochmal alle Aufgaben, die zu erledigen sind, gegeben und gefragt ob der Code so funktionieren kann. 

#### Antwort der KI: 

</details>

<details>
    <summary> Antwort anzeigen</summary>

```text
I’ve carefully gone through your codebase.
Right now, your implementation can rename files by applying regex/wildcard patterns and replacements, but it does not yet fully support all of the scenarios you listed:

✅ What your code already supports

Suffix/Prefix removal/addition
→ You can do this by defining wildcard + replacePattern in Files.RenameFiles.
Example: remove a suffix _old with

RenameFiles("path", "*_old.*", "$1.$2");


Partial expression renaming
→ Your Matcher.matcher lets you capture parts of filenames using * / ? and reinsert them.
So you can rename substrings or restructure names.

Moving number pads
→ Your ConvertDateInFilename already detects a numeric date pattern and reformats it (dd-MM-yy).
This is a form of "moving number pads."

❌ What’s missing / incomplete

Suffix/Prefix deletion in a generic way

Currently, you need to provide regex/wildcard yourself.

No helper function like RemovePrefix("ABC_") or RemoveSuffix("_XYZ").

General number re-ordering (pads)

You only handle dates (ddMMyy or dMMyy).

If you meant “move numbers to another position in filename” (e.g., img01_clipboard → clipboard_img01), that logic is not implemented.

Partial expression renaming with backreferences

In Matcher.matcher, your replacement with */? works, but is limited.

Standard regex backreferences ($1, $2) aren’t supported in your replacement patterns.

Integration is broken right now

In Program.cs, the Main is unfinished (Matcher. line).

Your tool doesn’t yet parse CLI arguments, so you can’t pass commands like

Renamer.exe "C:\files" "*.jpg" "new-*.jpg"


👉 So, your current code can already handle simple suffix/prefix changes, partial renaming, and a special case of moving number pads (dates).
But to make it a flexible renaming tool like you described, you’ll need:

A clean CLI interface in Program.cs.

Helper functions for prefix/suffix removal.

More general number handling (not just dates).

Proper support for regex groups/backreferences in replacements.

Do you want me to extend your code so that:

RenameFiles supports backreferences ($1, $2, etc.),

You can explicitly call helper methods like RemovePrefix, RemoveSuffix, and

Add a “MoveNumbers” function (reorder digits inside the filename)?

```
</details>

Weiter haben wir dann die KI genutzt um den unvollständigen Code zu verbessern und Fehler zu behebn, wobei die KI bei einfachen Fehlern, die jedoch nicht vom Compiler aufgezeigt wurden, oder nut teilweise und unverständlich. Oft haben wir uns dann auch die Probleme nachdem diese behoben wurden erklären lassen, denn ChatGPT kann durchaus, wenn ein Fehler dann mal lokalisiert ist, helfen Informationen zu filtern und Problematiken gut erklären. 

#### Frage an die KI: 

Can i Move number pads like this? :Renamer img-1.jpg 1-img.jpg?

### Antwort von ChatGPT: 

Good question 👍
Right now your code cannot do that yet, because:
Files.RenameFiles uses regex replace but doesn’t support backreferences like $1.
Your Matcher.matcher does support * / ? captures, but it isn’t wired into the console program.
🔎 Example you gave
Renamer img-1.jpg 1-img.jpg

Desired effect:
Input: img-1.jpg
Output: 1-img.jpg
Or with wildcard:
Renamer img-1.* 1-img.*
Input: img-1.jpg, img-1.png
Output: 1-img.jpg, 1-img.png
✅ How to make this work
We need to use Matcher.matcher in the console instead of plain Regex.Replace.

✅ Minimal fix
using System;
using System.IO;
using System.Collections.Generic;
using Renamer;   // <-- this makes Matcher visible

</details>

<details>
    <summary> Code anzeigen</summary>

```csharp
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
```
</details>



