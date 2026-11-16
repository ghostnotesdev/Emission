using Emission.Validation;

namespace Emission.Builders;
public class AssignmentBuilder : BuilderBase, IAssignmentBuilder
{
    private readonly BuilderProperty<AssignmentBuilder, IStatementBuilder, StatementBuilder> _assignment;

    public virtual IStatementBuilder Assignment => _assignment.Builder;

    public AssignmentBuilder()
    {
        _assignment = new(this);
    }

    public IAssignmentBuilder ConfigureAssignment(Action<IStatementBuilder> configure) => _assignment.Configure(configure);
    public IAssignmentBuilder UseAssignment(IStatementBuilder builder) => _assignment.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (Assignment is null)
            yield return new InvalidBuilderConfigurationDiagnostic(this, $".{nameof(UseAssignment)}() or .{nameof(ConfigureAssignment)}() must be called.");
        else
            foreach (var diag in PropagateDiagnostics(Assignment.Validate()))
                yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        icsb.EnsureSpace()
            .Append("=")
            .EnsureSpace();
        Assignment!.Build(icsb);
        icsb.EnsureSemicolon();
    }
}
