# Ki Dokumentation – FileRenamer  

## Generelle Beschreibung des Ablaufes:  

Die KI, die wir zum Ausarbeiten des Projektes genutzt haben, ist ChatGPT.  

Um uns einen generellen Überblick und einen ersten Startpunkt von der KI zu erhalten, haben wir die Aufgabenstellung im Ganzen kopiert und der KI als ersten Promt gegeben. Im weiteren Verlauf der Projektarbeit haben wir dann diesen Code, der natürlich noch nicht funktionsfähig war von der KI verbessern lassen und neue Ideen miteinfließen lassen. Im letzten Schritt haben wir dann mithilfe der KI kontrolliert, ob der Code, welchen wir geschrieben haben, so funktionieren kann. 

# KI Promts und Antworten  

### Startpromt // Stratpromt fehlt?  

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

####Antwort der KI: Perfect 👍 — let’s get your integration fixed first so the project actually runs from the console.

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

