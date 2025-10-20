kann I commit this as a compleated Readme???? :

File Renamer – C# Command Line Tool

A file renaming tool written in C# for batch renaming files using wildcard patterns, number formatting, and advanced name manipulation.
We implemented the core logic in Program.cs and Matcher.cs.
Adapted the regex patterns and wildcard conversion with the help of ChatGPT.
We tested and improved features such as automatic numbering, date recognition, and occurrence-based replacement.
---

Implemented Features

| Feature                             | Implemented  | Notes                                                         |                      
|-------------------------------------|--------------|---------------------------------------------------------------|
| Prefix/Suffix Removal               | yes          | img-123.jpg → 123.jpg                                         |
| Prefix/Suffix Change                | yes          | img-123.jpg → image-123.jpg                                   |
| Wildcard Matching (`*`, `?`)        | yes          | img-*.jpg → photo-*.jpg                                       |
| Add Leading Zeros                   | yes          | 1-img.jpg → 001-img.jpg                                       |
| Remove Leading Zeros                | yes          | 001-img.jpg → 1-img.jpg                                       |
| Renumber Files                      | yes          | img-abc.jpg, img-def.jpg → img-001.jpg, img-002.jpg           |
| Move Number Block                   | no           | Not implemented                                               |
| Insert Number Block                 | no           | Not implemented                                               |
| Remove Number Block                 | no           | Not implemented                                               |
| Date Format in Filenames            | yes          | 31122022test.jpg → 31-12-2022test.jpg                         |
| Replace Partial Expressions         | yes          | img → bild                                                    |
| Replace nth Occurrence              | yes          | e.g. only first occurrence replaced                           |
| Command-line Interface              | yes          | renamer.exe oldPattern newPattern                             |
| Help Page (`--help`)                | yes          | CLI shows usage when arguments are missing                    |
| Automated Tests                     | yes          | Unit tests included in Matcher.runTests()                     |

---

Usage Instructions
Run the tool using:
bash: renamer.exe <oldPattern> <newPattern> [occurrence]

- * = match any sequence of characters
- ? = match a single character
- [occurrence] = optional: replace only the nth occurrence of text

Example Commands:
renamer.exe img bild
renamer.exe data-*.jpg image-*.jpg
renamer.exe img-*.jpg img-001.jpg
renamer.exe ????????.jpg ??-??-????.jpg
renamer.exe img photo 1

Test Cases
Test 	Description			Command	Output
1	Replace "img" with "bild"	renamer.exe img bild	bild-test.jpg, bild-file.jpg
2	Wildcard prefix rename		renamer.exe data-*.jpg image-*.jpg	image-001.jpg, image-002.jpg
3	Add leading zeros		renamer.exe img-*.jpg img-001.jpg	img-001.jpg, img-002.jpg
4	Wildcard replace		renamer.exe abc-*.jpg neu-*.jpg	neu-test.jpg, neu-file.jpg
5	Format date in name		renamer.exe ????????.jpg ??-??-????.jpg	28-07-2013report.jpg, etc.
6	? wildcard matching		renamer.exe file?.jpg data?.jpg	data1.jpg, data2.jpg
7	Replace 1st occurrence		renamer.exe img photo 1	photo-img-test.jpg, etc.

Help Output
Usage: Renamer.exe <oldPattern> <newPattern> [occurrence]
Examples:
  renamer.exe img-*.jpg image-*.jpg
  renamer.exe img photo 1


Project Checklist:
- Source code committed to the main branch
- Teacher invited to the GitHub repository
- .exe file included in the release folder
- Scrum board maintained and documented
- AI usage clearly documented
- Tests implemented (including automated tests)
- Presentation prepared (code, tests, and demo)
