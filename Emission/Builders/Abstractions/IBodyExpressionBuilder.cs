namespace Emission.Builders;
public interface IBodyExpressionBuilder : IBuilder
{
    IEnumerable<IStatementBuilder> Statements { get; }
    IBodyExpressionBuilder UseStatements(IEnumerable<IStatementBuilder> statements);
    IBodyExpressionBuilder WithStatement(Action<IStatementBuilder> configure);
    IBodyExpressionBuilder WithStatement(IStatementBuilder builder);
    void Build(ICompilationStringBuilder icsb, bool newLine = false);
}
