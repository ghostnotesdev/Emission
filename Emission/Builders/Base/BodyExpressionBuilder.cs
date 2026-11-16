using Emission.Validation;

namespace Emission.Builders;
public class BodyExpressionBuilder : BuilderBase, IBodyExpressionBuilder
{
    private readonly BuilderEnumerableProperty<BodyExpressionBuilder, IStatementBuilder, StatementBuilder> _statements;
    public virtual IEnumerable<IStatementBuilder> Statements => _statements.Entries;

    public BodyExpressionBuilder()
    {
        _statements = new(this);
    }

    public IBodyExpressionBuilder UseStatements(IEnumerable<IStatementBuilder> statements) => _statements.Use(statements);
    public IBodyExpressionBuilder WithStatement(Action<IStatementBuilder> configure) => _statements.With(configure);
    public IBodyExpressionBuilder WithStatement(IStatementBuilder builder) => _statements.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (!Statements.Any())
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"At least one statement must be present");
        else
            foreach (var diag in PropagateDiagnostics(Statements.SelectMany(e => e.Validate())))
                yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb, bool newLine = false)
    {
        icsb.EnsureNewLine()
            .AppendLine("{")
            .IncreaseIndent();

        foreach (var statement in Statements)
        {
            statement.Build(icsb);
            icsb.EnsureSemicolon();
            icsb.EnsureNewLine();
        }

        icsb.DecreaseIndent()
            .EnsureNewLine()
            .Append("}");

        if (newLine)
            icsb.AppendLine();
    }
}
