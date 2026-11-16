namespace Emission.Builders;
public interface ICustomStatementBuilder : IBuilder
{
    void Build(ICompilationStringBuilder icsb);
}
