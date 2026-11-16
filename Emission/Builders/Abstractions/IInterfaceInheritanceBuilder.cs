namespace Emission.Builders;
public interface IInterfaceInheritanceBuilder : IBuilder, IInheritanceBuilder
{
    IInterfaceInheritanceBuilder UseImplementations(IEnumerable<ITypeBuilder> builders);
    IInterfaceInheritanceBuilder WithImplementation(Action<ITypeBuilder> configure);
    IInterfaceInheritanceBuilder WithImplementation(ITypeBuilder builder);
}
