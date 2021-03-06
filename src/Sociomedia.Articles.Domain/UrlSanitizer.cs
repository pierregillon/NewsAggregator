﻿using System.Text.RegularExpressions;

namespace Sociomedia.Articles.Domain
{
    public class UrlSanitizer
    {
        private static readonly string[] ImageFormatParameters = { "modified_at", "width", "height", "ratio_x", "ratio_y", "resize" };

        private static readonly Regex Regex = new Regex($"([&]?({string.Join('|', ImageFormatParameters)})=[^&]*&?)", RegexOptions.Compiled);

        public static string Sanitize(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) {
                return url;
            }
            var sanitizedUrl = Regex.Replace(url, string.Empty);
            return sanitizedUrl.EndsWith("?") ? sanitizedUrl.TrimEnd('?') : sanitizedUrl;
        }
    }
}