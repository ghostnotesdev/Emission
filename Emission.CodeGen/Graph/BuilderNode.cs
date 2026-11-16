using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Graph;

public sealed class BuilderNode
{
    public BuilderNode(
        INamedTypeSymbol @interface,
        IEnumerable<BuilderNode> implementations,
        IEnumerable<IMethodSymbol> methods)
    {
        Interface = @interface;
        Implementations = [.. implementations];
        Methods = [.. methods];
    }

    public INamedTypeSymbol Interface { get; }
    public IReadOnlyList<BuilderNode> Implementations { get; }
    public IReadOnlyList<IMethodSymbol> Methods { get; }

    public List<BuilderEdge> Children { get; } = [];
    public List<BuilderEdge> Parents { get; } = [];
}
