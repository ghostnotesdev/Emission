namespace Emission.Builders;
public interface IClassBuilder : ITopLevelBuilder<IClassBuilder>, ITypeBuilder
{
    IClassModifierBuilder ModifierBuilder { get; }
    IClassInheritanceBuilder InheritanceBuilder { get; }
    IGenericArgumentsBuilder GenericArgumentsBuilder { get; }
    IEnumerable<IEnumBuilder> Enums { get; }
    IEnumerable<IFieldBuilder> Fields { get; }
    IEnumerable<IPropertyBuilder> Properties { get; }
    IEnumerable<IConstructorBuilder> Constructors { get; }
    IEnumerable<IMethodBuilder> Methods { get; }
    IEnumerable<IClassBuilder> Classes { get; }

    IClassBuilder ConfigureModifiers(Action<IClassModifierBuilder> configure);
    IClassBuilder UseModifiers(IClassModifierBuilder builder);
    IClassBuilder UseEnums(IEnumerable<IEnumBuilder> builders);
    IClassBuilder WithEnum(Action<IEnumBuilder> configure);
    IClassBuilder WithEnum(IEnumBuilder builder);
    IClassBuilder UseFields(IEnumerable<IFieldBuilder> builders);
    IClassBuilder WithField(Action<IFieldBuilder> configure);
    IClassBuilder WithField(IFieldBuilder builder);
    IClassBuilder UseProperties(IEnumerable<IPropertyBuilder> builders);
    IClassBuilder WithProperty(Action<IPropertyBuilder> configure);
    IClassBuilder WithProperty(IPropertyBuilder builder);
    IClassBuilder UseConstructors(IEnumerable<IConstructorBuilder> builders);
    IClassBuilder WithConstructor(Action<IConstructorBuilder> configure);
    IClassBuilder WithConstructor(IConstructorBuilder builder);
    IClassBuilder UseMethods(IEnumerable<IMethodBuilder> builders);
    IClassBuilder WithMethod(Action<IMethodBuilder> configure);
    IClassBuilder WithMethod(IMethodBuilder builder);
    IClassBuilder UseClasses(IEnumerable<IClassBuilder> builders);
    IClassBuilder WithClass(Action<IClassBuilder> configure);
    IClassBuilder WithClass(IClassBuilder builder);
    IClassBuilder ConfigureInheritance(Action<IClassInheritanceBuilder> configure);
    IClassBuilder UseInheritance(IClassInheritanceBuilder builder);
    IClassBuilder ConfigureGenericArguments(Action<IGenericArgumentsBuilder> configure);
    IClassBuilder UseGenericArguments(IGenericArgumentsBuilder builder);
    void BuildClass(ICompilationStringBuilder icsb);
}
