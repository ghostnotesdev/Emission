namespace Emission.Builders;
public interface ITopLevelBuilder<TBuilder> : ITypeBuilder
    where TBuilder : ITopLevelBuilder<TBuilder>
{
    string Namespace { get; }
    string Name { get; }
    TBuilder UseNamespace(string ns);
    TBuilder UseName(string name);
}
