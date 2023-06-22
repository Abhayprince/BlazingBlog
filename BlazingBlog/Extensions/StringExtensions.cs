using System.Text.RegularExpressions;

namespace BlazingBlog.Extensions
{
    public static class StringExtensions
    {
        public static string Slugify(this string name) =>
            Regex.Replace(name.ToLower(), @"[^a-zA-Z0-9\-_]", "-", RegexOptions.Compiled, TimeSpan.FromSeconds(1))
                .Replace("--", "-")
                .Trim('-');
    }
}
