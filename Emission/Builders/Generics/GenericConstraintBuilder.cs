using Emission.Validation;

namespace Emission.Builders;
public class GenericConstraintBuilder : InheritanceBuilder<GenericConstraintBuilder>, IGenericConstraintBuilder
{
    private readonly SimpleBuilderProperty<GenericConstraintBuilder, GenericPrimaryConstraint> _primaryConstraint;
    private readonly BuilderSwitchProperty<GenericConstraintBuilder> _parameterlessConstructor;
    public GenericPrimaryConstraint PrimaryConstraint => _primaryConstraint.Value;
    public bool HasConstructorConstraint => _parameterlessConstructor.Value;
    public bool HasConstraints =>
        PrimaryConstraint != GenericPrimaryConstraint.None ||
        Types.Any() ||
        HasConstructorConstraint;

    public override string DiagnosticName => PrimaryConstraint.ToString();

    public GenericConstraintBuilder()
    {
        _primaryConstraint = new(this, GenericPrimaryConstraint.None);
        _parameterlessConstructor = new(this);
    }

    public IGenericConstraintBuilder UseConstraint(GenericPrimaryConstraint constraint) => _primaryConstraint.Use(constraint);
    public IGenericConstraintBuilder WithImplementation(Action<ITypeBuilder> configure) => WithType(configure);
    public IGenericConstraintBuilder WithImplementation(ITypeBuilder builder) => WithType(builder);
    public IGenericConstraintBuilder AsParameterlessConstructor() => _parameterlessConstructor.As();

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(base.Validate()))
            yield return diag;

        if (PrimaryConstraint is GenericPrimaryConstraint.Struct or GenericPrimaryConstraint.Unmanaged && HasConstructorConstraint)
        {
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Generic constraint '{PrimaryConstraint}' cannot be combined with new().");
        }
    }

    public override void Build(ICompilationStringBuilder icsb)
    {
        List<IStatementBuilder> constraints = [];

        if (PrimaryConstraint != GenericPrimaryConstraint.None)
        {
            constraints.Add(new StatementBuilder().UseStatement(PrimaryConstraint switch
            {
                GenericPrimaryConstraint.Class => "class",
                GenericPrimaryConstraint.NullableClass => "class?",
                GenericPrimaryConstraint.Struct => "struct",
                GenericPrimaryConstraint.NotNull => "notnull",
                GenericPrimaryConstraint.Unmanaged => "unmanaged",
                _ => throw new InvalidOperationException(
                    $"Unknown generic primary constraint '{PrimaryConstraint}'.")
            }));
        }

        constraints.AddRange(Types.Select(type => new StatementBuilder().UseStatement(type.BuildType)));

        if (HasConstructorConstraint)
            constraints.Add(new StatementBuilder().UseStatement("new()"));

        if (constraints.Count > 0)
        {
            icsb.EnsureSpace()
                .AppendWithSeparator(", ", constraints);
        }
    }
}

public enum GenericPrimaryConstraint
{
    None,
    Class,
    NullableClass,
    Struct,
    NotNull,
    Unmanaged
}
