using Emission.Types;
using Emission.Validation;

namespace Emission.Builders;
public class ArrayBuilder : BuilderBase, IArrayBuilder
{
    private readonly BuilderProperty<ArrayBuilder, ITypeBuilder, TypeBuilder> _entryType;
    private readonly BuilderEnumerableProperty<ArrayBuilder, IStatementBuilder, StatementBuilder> _entries;

    public virtual ITypeBuilder EntryType => _entryType.Builder;
    public virtual IEnumerable<IStatementBuilder> Entries => _entries.Entries;

    public ArrayBuilder()
    {
        _entryType = new(this);
        _entries = new(this);
    }

    public IArrayBuilder ConfigureType(Action<ITypeBuilder> configure) => _entryType.Configure(configure);
    public IArrayBuilder UseType(ITypeBuilder builder) => _entryType.Use(builder);
    public IArrayBuilder UseEntries(IEnumerable<IStatementBuilder> entries) => _entries.Use(entries);
    public IArrayBuilder WithEntry(Action<IStatementBuilder> configure) => _entries.With(configure);
    public IArrayBuilder WithEntry(IStatementBuilder builder) => _entries.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(EntryType.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Entries.SelectMany(e => e.Validate())))
            yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        icsb.Append("new ");
        EntryType.BuildType(icsb);
        if (EntryType.Reference is not TypeReference.Array)
            icsb.AppendLine("[]");
        else
            icsb.AppendLine();

        icsb.AppendLine("{");

        if (Entries.Any())
            icsb.IncreaseIndent()
                .AppendLinesWithSeparator(",", Entries)
                .DecreaseIndent();

        icsb.Append("}");
    }
}
