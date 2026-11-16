namespace Emission.Builders;
public interface IInterfaceBuilder : ITopLevelBuilder<IInterfaceBuilder>, ITypeBuilder
{
    IInterfaceModifierBuilder ModifierBuilder { get; }
    IInterfaceInheritanceBuilder InheritanceBuilder { get; }
    IEnumerable<IPropertyBuilder> Properties { get; }
    IEnumerable<IMethodBuilder> Methods { get; }

    IInterfaceBuilder ConfigureModifiers(Action<IInterfaceModifierBuilder> configure);
    IInterfaceBuilder UseModifiers(IInterfaceModifierBuilder builder);
    IInterfaceBuilder ConfigureInheritance(Action<IInterfaceInheritanceBuilder> configure);
    IInterfaceBuilder UseInheritance(IInterfaceInheritanceBuilder builder);
    IInterfaceBuilder UseProperties(IEnumerable<IPropertyBuilder> builders);
    IInterfaceBuilder WithProperty(Action<IPropertyBuilder> configure);
    IInterfaceBuilder WithProperty(IPropertyBuilder builder);
    IInterfaceBuilder UseMethods(IEnumerable<IMethodBuilder> builders);
    IInterfaceBuilder WithMethod(Action<IMethodBuilder> configure);
    IInterfaceBuilder WithMethod(IMethodBuilder builder);
    void BuildInterface(ICompilationStringBuilder icsb);
}
