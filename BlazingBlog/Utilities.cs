namespace BlazingBlog
{
    public static class Utilities
    {
        private static readonly string[] _colorClasses = new string[] { "primary", "success", "info", "danger", "warning", "dark" };
        public static string GetRandomColorClass() =>
            _colorClasses.OrderBy(c=> Guid.NewGuid()).First();

        public static string GetInitials(string text)
        {
            const string DefaultInitials = "BB";
            if(!string.IsNullOrEmpty(text))
            {
                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if(parts.Length > 1)
                {
                    return $"{parts[0][0]}{parts[1][0]}";
                }
                else
                {
                    return text.Length > 1 ? text[..2] : text;
                }
            }
            return DefaultInitials;
        }
    }
}
