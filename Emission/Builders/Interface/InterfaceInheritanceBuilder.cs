
namespace Emission.Builders;
public class InterfaceInheritanceBuilder : InheritanceBuilder<InterfaceInheritanceBuilder>, IInterfaceInheritanceBuilder
{
    public IInterfaceInheritanceBuilder UseImplementations(IEnumerable<ITypeBuilder> builders) => UseTypes(builders);
    public IInterfaceInheritanceBuilder WithImplementation(Action<ITypeBuilder> configure) => WithType(configure);
    public IInterfaceInheritanceBuilder WithImplementation(ITypeBuilder builder) => WithType(builder);
}
