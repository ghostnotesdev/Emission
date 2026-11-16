using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace Emission.Extensions;
public static class StringExtensions
{
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (input.Length == 1)
            return input.ToLowerInvariant();

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var words = input.Trim().Split(['_', '-', ' '], StringSplitOptions.RemoveEmptyEntries);

        var result = new StringBuilder();
        foreach (var word in words)
        {
            if (word.Length == 0) continue;

            result.Append(char.ToUpperInvariant(word[0]));
            if (word.Length > 1)
                result.Append(word.Substring(1));
        }

        return result.ToString();
    }

    public static bool IsValidIdentifier(this string? value)
    {
        return !(string.IsNullOrWhiteSpace(value) || !SyntaxFacts.IsValidIdentifier(value));
    }
}
