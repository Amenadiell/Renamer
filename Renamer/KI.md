# Ki Dokumentation – FileRenamer  

## Generelle Beschreibung des Ablaufes:  

Die KI, die wir zum Ausarbeiten des Projektes genutzt haben, ist ChatGPT.  

Um uns einen generellen Überblick und einen ersten Startpunkt von der KI zu erhalten, haben wir die Aufgabenstellung im Ganzen kopiert und der KI als ersten Promt gegeben. Im weiteren Verlauf der Projektarbeit haben wir dann diesen Code, der natürlich noch nicht funktionsfähig war von der KI verbessern lassen und neue Ideen miteinfließen lassen. Im letzten Schritt haben wir dann mithilfe der KI kontrolliert, ob der Code, welchen wir geschrieben haben, so funktionieren kann. 

# KI Promts und Antworten  

#### Startpromt // Stratpromt fehlt?  

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

```text
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


