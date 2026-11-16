namespace Emission.Builders;
public interface IInheritanceBuilder : IBuilder
{
    IEnumerable<ITypeBuilder> Types { get; }
    void Build(ICompilationStringBuilder icsb);
}
