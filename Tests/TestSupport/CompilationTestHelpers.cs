using Emission.Builders;
using Emission.Validation;

namespace Emission.UnitTests.TestSupport;

internal static class CompilationTestHelpers
{
    public static string Build(Action<ICompilationStringBuilder> build)
    {
        var builder = new CompilationStringBuilder();
        build(builder);
        return builder.ToString();
    }

    public static string[] Messages(IBuilder builder) => [.. builder.Validate().Select(diagnostic => diagnostic.Message)];

    public static string BuildBuilder(IBuilder builder, Action<ICompilationStringBuilder> build)
    {
        _ = builder;
        return Build(build);
    }

    public static (string LowLevel, string Convenience) BuildPair(
        Action<ICompilationStringBuilder> lowLevel,
        Action<ICompilationStringBuilder> convenience)
        => (Build(lowLevel), Build(convenience));
}
