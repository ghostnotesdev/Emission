namespace Emission.Builders;
public readonly struct BuiltSource(string hintName, string source)
{
    public string HintName { get; } = hintName;
    public string Source { get; } = source;
}
