using Emission.Validation;

namespace Emission.Builders;
public class ExpressionBuilder : BuilderBase, IExpressionBuilder
{
    private readonly BuilderProperty<ExpressionBuilder, IStatementBuilder, StatementBuilder> _expression;

    public virtual IStatementBuilder Expression => _expression.Builder;

    public ExpressionBuilder()
    {
        _expression = new(this);
    }

    public IExpressionBuilder ConfigureExpression(Action<IStatementBuilder> configure) => _expression.Configure(configure);
    public IExpressionBuilder UseExpression(IStatementBuilder builder) => _expression.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (Expression is null)
            yield return new InvalidBuilderConfigurationDiagnostic(this, $".{nameof(UseExpression)}() must be called.");
        else
            foreach (var diag in PropagateDiagnostics(Expression.Validate()))
                yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb, bool newLine = false)
    {
        icsb.EnsureSpace()
            .Append("=>");
        if (newLine)
            icsb.IncreaseIndent().EnsureNewLine();
        else
            icsb.EnsureSpace();

        Expression!.Build(icsb);
        icsb.EnsureSemicolon();
        if (newLine)
            icsb.DecreaseIndent();
    }
}
