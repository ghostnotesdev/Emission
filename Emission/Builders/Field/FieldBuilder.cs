using Emission.Validation;

namespace Emission.Builders;
public class FieldBuilder : BuilderBase, IFieldBuilder
{
    private readonly BuilderIdentifierProperty<FieldBuilder> _fieldName;
    private readonly BuilderProperty<FieldBuilder, ITypeBuilder, TypeBuilder> _type;
    private readonly BuilderProperty<FieldBuilder, IFieldModifierBuilder, FieldModifierBuilder> _modifier;
    private readonly OptionalBuilderProperty<FieldBuilder, IAssignmentBuilder, AssignmentBuilder> _assignment;

    public virtual string FieldName => _fieldName.Value;
    public virtual ITypeBuilder TypeBuilder => _type.Builder;
    public virtual IFieldModifierBuilder ModifierBuilder => _modifier.Builder;
    public virtual IAssignmentBuilder? Assignment => _assignment.Builder;
    
    public override string DiagnosticName => FieldName ?? "<unnamed field>";

    public FieldBuilder()
    {
        _type = new(this);
        _modifier = new(this);
        _assignment = new(this);
        _fieldName = new(this);
    }

    public IFieldBuilder ConfigureModifiers(Action<IFieldModifierBuilder> configure) => _modifier.Configure(configure);
    public IFieldBuilder UseModifiers(IFieldModifierBuilder builder) => _modifier.Use(builder);
    public IFieldBuilder UseName(string fieldName) => _fieldName.Use(fieldName);
    public IFieldBuilder ConfigureType(Action<ITypeBuilder> configure) => _type.Configure(configure);
    public IFieldBuilder UseType(ITypeBuilder builder) => _type.Use(builder);
    public IFieldBuilder ConfigureAssignment(Action<IAssignmentBuilder> configure) => _assignment.Configure(configure);
    public IFieldBuilder UseAssignment(IAssignmentBuilder builder) => _assignment.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _fieldName.Validate(FieldName, nameof(FieldName)))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(ModifierBuilder.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(TypeBuilder.Validate()))
            yield return diag;

        if (Assignment is not null)
            foreach (var diag in PropagateDiagnostics(Assignment.Validate()))
                yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        ModifierBuilder.Build(icsb);
        icsb.EnsureSpace();
        TypeBuilder.BuildType(icsb);
        icsb.EnsureSpace();
        icsb.Append(FieldName);
        Assignment?.Build(icsb);
        icsb.EnsureSemicolon();
        icsb.AppendLine();
    }
}
