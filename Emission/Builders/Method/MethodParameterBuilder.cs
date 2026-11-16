using Emission.Validation;

namespace Emission.Builders;
public class MethodParameterBuilder : BuilderBase, IMethodParameterBuilder
{
    private readonly BuilderIdentifierProperty<MethodParameterBuilder> _name;
    private readonly BuilderProperty<MethodParameterBuilder, ITypeBuilder, TypeBuilder> _type;

    public virtual string ParameterName => _name.Value;
    public virtual ITypeBuilder ParameterType => _type.Builder;

    public override string DiagnosticName => ParameterName ?? "<unnamed parameter>";

    public MethodParameterBuilder()
    {
        _name = new(this);
        _type = new(this);
    }

    public IMethodParameterBuilder UseName(string propertyName) => _name.Use(propertyName);
    public IMethodParameterBuilder ConfigureType(Action<ITypeBuilder> configure) => _type.Configure(configure);
    public IMethodParameterBuilder UseType(ITypeBuilder builder) => _type.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _name.Validate(ParameterName, nameof(ParameterName)))
            yield return diag;

        foreach (var diag in ParameterType.Validate())
            yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        ParameterType.BuildType(icsb);
        icsb.EnsureSpace()
            .Append(ParameterName);
    }
}
