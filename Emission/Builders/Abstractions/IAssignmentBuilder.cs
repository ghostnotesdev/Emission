namespace Emission.Builders;
public interface IAssignmentBuilder : IBuilder
{
    IStatementBuilder Assignment { get; }
    IAssignmentBuilder ConfigureAssignment(Action<IStatementBuilder> configure);
    IAssignmentBuilder UseAssignment(IStatementBuilder builder);
    void Build(ICompilationStringBuilder icsb);
}
