using System;
using System.Text.RegularExpressions;

namespace CodeMap.Documentation
{
    public static class Extensions
    {
        public static string CollapseIndentation(this string value)
        {
            var trimmedValue = value.Trim('\r', '\n');
            var match = Regex.Match(trimmedValue, @"^\s+", RegexOptions.Multiline);
            if (match.Success)
            {
                var indentation = match.Value.Length;
                do
                {
                    indentation = Math.Min(indentation, match.Value.Length);
                    match = match.NextMatch();
                } while (match.Success);
                var collapsedText = Regex.Replace(trimmedValue, @$"^\s{{{indentation}}}", string.Empty, RegexOptions.Multiline);
                return collapsedText;
            }
            else
                return trimmedValue;
        }
    }
}