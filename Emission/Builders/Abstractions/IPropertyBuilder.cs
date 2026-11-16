namespace Emission.Builders;
public interface IPropertyBuilder : IBuilder
{
    bool IsInterfaceProperty { get; }
    string PropertyName { get; }
    IPropertyModifierBuilder Modifier { get; }
    ITypeBuilder PropertyType { get; }
    IGetterBuilder Getter { get; }
    ISetterBuilder Setter { get; }
    IAssignmentBuilder? Assignment { get; }

    IPropertyBuilder ConfigureModifiers(Action<IPropertyModifierBuilder> configure);
    IPropertyBuilder UseModifiers(IPropertyModifierBuilder builder);
    IPropertyBuilder UseName(string propertyName);
    IPropertyBuilder ConfigureType(Action<ITypeBuilder> configure);
    IPropertyBuilder UseType(ITypeBuilder builder);
    IPropertyBuilder ConfigureGetter(Action<IGetterBuilder> configure);
    IPropertyBuilder UseGetter(IGetterBuilder builder);
    IPropertyBuilder ConfigureSetter(Action<ISetterBuilder> configure);
    IPropertyBuilder UseSetter(ISetterBuilder builder);
    IPropertyBuilder ConfigureAssignment(Action<IAssignmentBuilder> configure);
    IPropertyBuilder UseAssignment(IAssignmentBuilder builder);
    IPropertyBuilder AsInterfaceProperty();
    void BuildContract(ICompilationStringBuilder icsb);
    void BuildImplementation(ICompilationStringBuilder icsb);
}
