﻿using DND.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToUrlSlug(this string s)
        {
            return UrlSlugger.ToUrlSlug(s);
        }

        public static string CamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (!char.IsUpper(s[0]))
                return s;
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }
            return new string(chars);
        }

        public static string PascalCase(this string s)
        {
            // If there are 0 or 1 characters, just return the string.
            if (s == null) return s;
            if (s.Length < 2) return s.ToUpper();

            // Split the string into words.
            string[] words = s.Split(
                new char[] { ' ', '-', '_' },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }


        public static string GetStringWithSpacesAndFirstLetterUpper(this string input)
        {
            var value = Regex.Replace(
               input,
               "(?<!^)" +
               "(" +
               "  [A-Z][a-z] |" +
               "  (?<=[a-z])[A-Z] |" +
               "  (?<![A-Z])[A-Z]$" +
               ")",
               " $1",
               RegexOptions.IgnorePatternWhitespace);

            value = value.Replace("-", " ").Replace("_", " ");

            var chars = value.ToCharArray();
            chars[0] = char.ToUpper(chars[0], CultureInfo.InvariantCulture);

            return new string(chars); ;
        }

        public static string ReplaceFromDictionary(this string s, Dictionary<string, string> dict)
        {
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                s = s.Replace(kvp.Key, kvp.Value);
            }
            return s;
        }

        public static string ToQueryString(this Dictionary<string, string> dict)
        {
            var builder = new StringBuilder();
            var count = 0;
            foreach (KeyValuePair<string, string> kvp in dict)
            {

                builder.Append(System.Net.WebUtility.UrlEncode(kvp.Key) + "=" + System.Net.WebUtility.UrlEncode(kvp.Value));
                if (count != dict.Keys.Count - 1)
                {
                    builder.Append("&");
                }
                count++;

            }
            return builder.ToString();
        }

        public static string Truncate(this string source, int length)
        {
            if (source != null)
            {
                if (source.Length > length)
                {
                    source = source.Substring(0, length) + "...";
                }
            }
            return source;
        }

        public static int CountStringOccurrences(this string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

    }
}
