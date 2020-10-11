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

        public static string ToSemver(this Version version)
        {
            var prerelease = string.Empty;
            if (version.Build > 0)
                switch (version.Build / 1000)
                {
                    case 1:
                        prerelease = "-alpha" + version.Build % 1000;
                        break;

                    case 2:
                        prerelease = "-beta" + version.Build % 1000;
                        break;

                    case 3:
                        prerelease = "-rc" + version.Build % 1000;
                        break;
                }

            return $"{version.Major}.{version.Minor}.{version.Revision}{prerelease}";
        }
    }
}