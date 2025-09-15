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