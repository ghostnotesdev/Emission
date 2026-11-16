using Emission.Validation;

namespace Emission.Builders;
public class StatementBuilder : BuilderBase, IStatementBuilder
{
    private readonly StatementBuilderProperty _statement;

    public virtual string? StringStatement => _statement.StringStatement;
    public virtual CompilationExpression? DeferredStatement => _statement.DeferredStatement;
    public virtual ICustomStatementBuilder? CustomStatement => _statement.CustomStatement;

    public StatementBuilder()
    {
        _statement = new(this);
    }

    public IStatementBuilder ConfigureStatement<TStatementBuilder>(Action<TStatementBuilder> configure)
        where TStatementBuilder : ICustomStatementBuilder, new() => _statement.ConfigureStatement(configure);
    public IStatementBuilder UseStatement(string statement) => _statement.UseStatement(statement);
    public IStatementBuilder UseStatement(CompilationExpression statement) => _statement.UseStatement(statement);
    public IStatementBuilder UseStatement(ICustomStatementBuilder statement) => _statement.UseStatement(statement);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (StringStatement is null && DeferredStatement is null && CustomStatement is null)
            yield return new InvalidBuilderConfigurationDiagnostic(this, $".{nameof(UseStatement)}() or .{nameof(ConfigureStatement)}() must be called.");

        if (StringStatement is not null && string.IsNullOrWhiteSpace(StringStatement))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"{nameof(StringStatement)} cannot be empty or white space.");

        if (CustomStatement is not null)
            foreach (var diag in PropagateDiagnostics(CustomStatement.Validate()))
                yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        if (StringStatement is not null)
            icsb.Append(StringStatement);
        else if (DeferredStatement is not null)
            DeferredStatement.Invoke(icsb);
        else
            CustomStatement?.Build(icsb);
    }
}