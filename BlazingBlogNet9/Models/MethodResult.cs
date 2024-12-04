namespace BlazingBlog.Models
{
    public record struct MethodResult(bool Status, string? ErrorMessage = null)
    {
        public static MethodResult Succes() => new(true);
        public static MethodResult Failure(string errorMessage) => new(false, errorMessage);
    }
}
