using Emission.Validation;

namespace Emission.Builders;
public class GenericArgumentBuilder : BuilderBase, IGenericArgumentBuilder
{
    private readonly BuilderIdentifierProperty<GenericArgumentBuilder> _name;
    private readonly BuilderProperty<GenericArgumentBuilder, IGenericConstraintBuilder, GenericConstraintBuilder> _constraints;

    public virtual string ArgumentName => _name.Value;
    public virtual IGenericConstraintBuilder Constraints => _constraints.Builder;

    public override string DiagnosticName => ArgumentName ?? "<unnamed generic argument>";

    public GenericArgumentBuilder()
    {
        _name = new(this);
        _constraints = new(this);
    }

    public IGenericArgumentBuilder UseName(string name) => _name.Use(name);
    public IGenericArgumentBuilder ConfigureConstraints(Action<IGenericConstraintBuilder> configure) => _constraints.Configure(configure);
    public IGenericArgumentBuilder UseConstraints(IGenericConstraintBuilder builder) => _constraints.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _name.Validate(ArgumentName, nameof(ArgumentName)))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Constraints.Validate()))
            yield return diag;
    }

    public void BuildDeclaration(ICompilationStringBuilder icsb)
    {
        icsb.Append(ArgumentName);
    }

    public void BuildConstraints(ICompilationStringBuilder icsb)
    {
        if (!Constraints.HasConstraints)
            return;

        icsb.EnsureSpace()
            .Append("where")
            .EnsureSpace()
            .Append(ArgumentName!)
            .EnsureSpace()
            .Append(":");

        Constraints.Build(icsb);
    }
}
