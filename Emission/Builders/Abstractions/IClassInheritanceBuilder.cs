namespace Emission.Builders;
public interface IClassInheritanceBuilder : IBuilder, IInheritanceBuilder
{
    ITypeBuilder? BaseClass { get; }
    IClassInheritanceBuilder ConfigureBaseClass(Action<ITypeBuilder> configure);
    IClassInheritanceBuilder UseBaseClass(ITypeBuilder builder);
    IClassInheritanceBuilder UseImplementations(IEnumerable<ITypeBuilder> builders);
    IClassInheritanceBuilder WithImplementation(Action<ITypeBuilder> configure);
    IClassInheritanceBuilder WithImplementation(ITypeBuilder builder);
}
