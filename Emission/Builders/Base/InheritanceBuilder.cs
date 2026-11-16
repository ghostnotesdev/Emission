using Emission.Validation;

namespace Emission.Builders;
public abstract class InheritanceBuilder<TImplementation> : BuilderBase, IInheritanceBuilder
    where TImplementation : InheritanceBuilder<TImplementation>
{
    private readonly BuilderEnumerableProperty<TImplementation, ITypeBuilder, TypeBuilder> _types;

    public virtual IEnumerable<ITypeBuilder> Types => _types.Entries;

    public InheritanceBuilder()
    {
        _types = new((TImplementation)this);
    }

    protected TImplementation UseTypes(IEnumerable<ITypeBuilder> types) => _types.Use(types);
    protected TImplementation WithType(Action<ITypeBuilder> configure) => _types.With(configure);
    protected TImplementation WithType(ITypeBuilder builder) => _types.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(Types.SelectMany(t => t.Validate())))
            yield return diag;
    }

    public virtual void Build(ICompilationStringBuilder icsb)
    {
        if (!Types.Any())
            return;

        icsb.EnsureSpace()
            .Append(": ")
            .AppendWithSeparator(", ", Types
                .Select(typeBuilder => new StatementBuilder()
                    .UseStatement(typeBuilder.BuildType)));
    }
}
