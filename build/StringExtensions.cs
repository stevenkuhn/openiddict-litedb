static class StringExtensions
{
    public static (string T1, string T2) Split(this string value, char separator)
    {
        var result = value.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return (result[0], result.Length > 1 ? result[1] : string.Empty);
    }
}