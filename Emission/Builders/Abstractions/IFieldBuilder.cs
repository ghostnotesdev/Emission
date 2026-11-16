namespace Emission.Builders;
public interface IFieldBuilder : IBuilder
{
    ITypeBuilder TypeBuilder { get; }
    IFieldModifierBuilder ModifierBuilder { get; }
    IAssignmentBuilder? Assignment { get; }
    string FieldName { get; }
    IFieldBuilder ConfigureModifiers(Action<IFieldModifierBuilder> configure);
    IFieldBuilder UseModifiers(IFieldModifierBuilder builder);
    IFieldBuilder UseName(string fieldName);
    IFieldBuilder ConfigureType(Action<ITypeBuilder> configure);
    IFieldBuilder UseType(ITypeBuilder builder);
    IFieldBuilder ConfigureAssignment(Action<IAssignmentBuilder> configure);
    IFieldBuilder UseAssignment(IAssignmentBuilder builder);
    void Build(ICompilationStringBuilder icsb);
}
