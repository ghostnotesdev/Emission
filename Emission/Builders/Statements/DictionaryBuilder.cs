using Emission.Validation;

namespace Emission.Builders;
public class DictionaryBuilder : BuilderBase, IDictionaryBuilder
{
    private readonly BuilderProperty<DictionaryBuilder, ITypeBuilder, TypeBuilder> _dictionary;
    private readonly SimpleBuilderEnumerableProperty<DictionaryBuilder, DictionaryEntry> _entries;

    public virtual ITypeBuilder DictionaryType => _dictionary.Builder;
    public virtual IEnumerable<DictionaryEntry> Entries => _entries.Entries;

    public DictionaryBuilder()
    {
        _dictionary = new(this);
        _entries = new(this);
    }

    public IDictionaryBuilder ConfigureType(Action<ITypeBuilder> configure) => _dictionary.Configure(configure);
    public IDictionaryBuilder UseType(ITypeBuilder builder) => _dictionary.Use(builder);
    public IDictionaryBuilder UseEntries(IEnumerable<DictionaryEntry> entries) => _entries.Use(entries);
    public IDictionaryBuilder WithEntry(DictionaryEntry entry) => _entries.With(entry);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(DictionaryType.Validate()))
            yield return diag;

        foreach (var entry in Entries)
        {
            foreach (var diag in entry.Value.Validate())
                yield return diag;

            foreach (var diag in entry.Key.Validate())
                yield return diag;
        }
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        var dictionary = new ObjectBuilder()
            .UseType(DictionaryType);

        dictionary.Build(icsb);
        if (!Entries.Any())
            return;

        icsb.AppendLine()
            .AppendLine("{")
            .IncreaseIndent()
            .AppendLinesWithSeparator(",", Entries
            .Select(entry => GetDictionaryEntry(entry)))
            .DecreaseIndent()
            .Append("}");
    }

    private IStatementBuilder GetDictionaryEntry(DictionaryEntry entry) => new StatementBuilder()
        .UseStatement(new CompilationExpression(builder =>
        {
            builder.Append("{ ");
            entry.Key.Build(builder);
            builder.Append(", ");
            entry.Value.Build(builder);
            builder.Append(" }");
        }));
}

public readonly struct DictionaryEntry
{
    public readonly IStatementBuilder Key;
    public readonly IStatementBuilder Value;

    public DictionaryEntry(IStatementBuilder key, IStatementBuilder value)
    {
        Key = key;
        Value = value;
    }

    public DictionaryEntry(Action<IStatementBuilder> key, Action<IStatementBuilder> value)
    {
        var keyBuilder = new StatementBuilder();
        key.Invoke(keyBuilder);
        Key = keyBuilder;
        var valueBuilder = new StatementBuilder();
        value.Invoke(valueBuilder);
        Value = valueBuilder;
    }
}
