using Emission.Validation;

namespace Emission.Builders;
public class StatementParameterBuilder : BuilderBase, IStatementParameterBuilder
{
    private readonly BuilderProperty<StatementParameterBuilder, ITypeBuilder, TypeBuilder> _parameterType;
    private readonly BuilderProperty<StatementParameterBuilder, IStatementBuilder, StatementBuilder> _statement;

    public virtual ITypeBuilder ParameterType => _parameterType.Builder;
    public virtual IStatementBuilder Statement => _statement.Builder;

    public StatementParameterBuilder()
    {
        _parameterType = new(this);
        _statement = new(this);
    }

    public IStatementParameterBuilder ConfigureType(Action<ITypeBuilder> configure) => _parameterType.Configure(configure);
    public IStatementParameterBuilder UseType(ITypeBuilder builder) => _parameterType.Use(builder);
    public IStatementParameterBuilder ConfigureStatement(Action<IStatementBuilder> configure) => _statement.Configure(configure);
    public IStatementParameterBuilder UseStatement(IStatementBuilder builder) => _statement.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(ParameterType.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Statement.Validate()))
            yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        Statement.Build(icsb);
    }
}
