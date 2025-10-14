# FileRenamer - Tests

## Test 1: Text ersetzen

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul img-test.jpg
copy /Y nul img-file.jpg

```

**Befehl:**
```cmd
FileRenamer img bild
```

**Ergebnis:** `bild-test.jpg`, `bild-file.jpg`

---

## Test 2: Prefix ändern

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul data-001.jpg
copy /Y nul data-002.jpg

```

**Befehl:**
```cmd
FileRenamer data-*.jpg image-*.jpg
```

**Ergebnis:** `image-001.jpg`, `image-002.jpg`

---

## Test 3: Nummerierung

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul img-abc.jpg
copy /Y nul img-def.jpg

```

**Befehl:**
```cmd
FileRenamer img-*.jpg img-001.jpg
```

**Ergebnis:** `img-001.jpg`, `img-002.jpg`

---

## Test 4: Wildcard

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul abc-test.jpg
copy /Y nul abc-file.jpg

```

**Befehl:**
```cmd
Simon```

**Ergebnis:** `neu-test.jpg`, `neu-file.jpg`

---

## Test 5: Datum formatieren

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul 28072013report.jpg
copy /Y nul 15082024notes.jpg

```

**Befehl:**
```cmd
FileRenamer ????????*.jpg ??-??-????*.jpg
```

**Ergebnis:** `28-07-2013report.jpg`, `15-08-2024notes.jpg`

---

## Test 6: Fragezeichen

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul file1.jpg
copy /Y nul file2.jpg
copy /Y nul file10.jpg

```

**Befehl:**
```cmd
FileRenamer file?.jpg data?.jpg
```

**Ergebnis:** `data1.jpg`, `data2.jpg` (file10.jpg bleibt)

---

## Test 7: Erstes Vorkommen

**Dateien erstellen:**
```cmd
del /Q *.jpg 2>nul
copy /Y nul img-img-test.jpg
copy /Y nul img-file-img.jpg

```

**Befehl:**
```cmd
FileRenamer img photo 1
```

**Ergebnis:** `photo-img-test.jpg`, `photo-file-img.jpg`

---

## Aufräumen

```cmd
del /Q *.jpg 2>nul
```
