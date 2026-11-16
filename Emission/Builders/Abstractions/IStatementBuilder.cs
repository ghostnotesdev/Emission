
namespace Emission.Builders;
public interface IStatementBuilder : IBuilder
{
    string? StringStatement { get; }
    CompilationExpression? DeferredStatement { get; }
    ICustomStatementBuilder? CustomStatement { get; }

    IStatementBuilder ConfigureStatement<TStatementBuilder>(Action<TStatementBuilder> configure) where TStatementBuilder : ICustomStatementBuilder, new();
    IStatementBuilder UseStatement(string statement);
    IStatementBuilder UseStatement(CompilationExpression statement);
    IStatementBuilder UseStatement(ICustomStatementBuilder statement);
    void Build(ICompilationStringBuilder icsb);
}
