using Emission.Validation;

namespace Emission.Builders;
public class CollectionBuilder : BuilderBase, ICollectionBuilder
{
    private readonly BuilderProperty<CollectionBuilder, ITypeBuilder, TypeBuilder> _collectionType;
    private readonly BuilderProperty<CollectionBuilder, ITypeBuilder, TypeBuilder> _entryType;
    private readonly BuilderEnumerableProperty<CollectionBuilder, IStatementBuilder, StatementBuilder> _entries;

    public virtual ITypeBuilder CollectionType => _collectionType.Builder;
    public virtual ITypeBuilder EntryType => _entryType.Builder;
    public virtual IEnumerable<IStatementBuilder> Entries => _entries.Entries;

    public CollectionBuilder()
    {
        _collectionType = new(this);
        _entryType = new(this);
        _entries = new(this);
    }

    public ICollectionBuilder ConfigureCollectionType(Action<ITypeBuilder> configure) => _collectionType.Configure(configure);
    public ICollectionBuilder UseCollectionType(ITypeBuilder builder) => _collectionType.Use(builder);
    public ICollectionBuilder ConfigureEntryType(Action<ITypeBuilder> configure) => _entryType.Configure(configure);
    public ICollectionBuilder UseEntryType(ITypeBuilder builder) => _entryType.Use(builder);
    public ICollectionBuilder UseEntries(IEnumerable<IStatementBuilder> builders) => _entries.Use(builders);
    public ICollectionBuilder WithEntry(Action<IStatementBuilder> configure) => _entries.With(configure);
    public ICollectionBuilder WithEntry(IStatementBuilder builder) => _entries.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(CollectionType.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(EntryType.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Entries.SelectMany(e => e.Validate())))
            yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        icsb.Append("new ");
        CollectionType.BuildType(icsb);
        icsb.Append("(")
            .Append("new ");
        EntryType.BuildType(icsb);
        icsb.AppendLine("[]")
            .AppendLine("{");

        if (Entries.Any())
            icsb.IncreaseIndent()
                .AppendLinesWithSeparator(",", Entries)
                .DecreaseIndent();

        icsb.Append("})");
    }
}
