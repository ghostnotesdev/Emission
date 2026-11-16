namespace Emission.Builders;
public interface IExpressionBuilder : IBuilder
{
    IStatementBuilder? Expression { get; }
    IExpressionBuilder ConfigureExpression(Action<IStatementBuilder> configure);
    IExpressionBuilder UseExpression(IStatementBuilder builder);
    void Build(ICompilationStringBuilder icsb, bool newLine = false);
}
