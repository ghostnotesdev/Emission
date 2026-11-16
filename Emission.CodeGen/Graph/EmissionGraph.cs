using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Graph;

public class EmissionGraph
{
    private readonly Dictionary<INamedTypeSymbol, BuilderNode> _nodes = new(SymbolEqualityComparer.Default);

    public EmissionGraph(INamespaceSymbol buildersNamespace, INamedTypeSymbol builderInterface)
    {
        BuildGraph(buildersNamespace, builderInterface);
    }

    public IReadOnlyDictionary<INamedTypeSymbol, BuilderNode> Graph => _nodes;
    public IEnumerable<BuilderNode> Nodes => _nodes.Values;

    public IEnumerable<BuilderEdge> GetConsumers(INamedTypeSymbol builder)
    {
        if (!_nodes.TryGetValue(builder, out var node))
            return [];

        return node.Parents;
    }

    private void BuildGraph(INamespaceSymbol buildersNamespace, INamedTypeSymbol builderInterface)
    {
        IEnumerable<INamedTypeSymbol> builderInterfaces = buildersNamespace
            .GetTypeMembers()
            .Where(type => type.TypeKind == TypeKind.Interface && type.AllInterfaces.Contains(builderInterface, SymbolEqualityComparer.Default));

        // Pass 1: construct all nodes.
        foreach (INamedTypeSymbol builder in builderInterfaces)
        {
            _nodes.Add(builder, GetNode(builder, builderInterfaces));
        }

        // Pass 2: connect them.
        foreach (BuilderNode parent in _nodes.Values)
        {
            foreach (IMethodSymbol method in parent.Methods)
            {
                foreach ((INamedTypeSymbol dependency, bool isAction) in GetBuilderDependencies(method))
                {
                    if (!_nodes.TryGetValue(dependency, out BuilderNode? child))
                        continue;

                    (string prefix, string subject) = SplitMethodName(method.Name);
                    BuilderEdge edge = new(parent, child, method, prefix, subject, isAction);
                    parent.Children.Add(edge);
                    child.Parents.Add(edge);
                }
            }
        }
    }

    private BuilderNode GetNode(INamedTypeSymbol builder, IEnumerable<INamedTypeSymbol> builderInterfaces)
    {
        if (_nodes.TryGetValue(builder, out BuilderNode? node))
            return node;

        IEnumerable<BuilderNode> implementations = builderInterfaces
            .Where(type => type.AllInterfaces.Any(
                inherited => SymbolEqualityComparer.Default.Equals(
                    inherited.OriginalDefinition,
                    builder.OriginalDefinition)))
            .Select(type => GetNode(type, builderInterfaces));

        var methods = builder
            .GetMembers()
            .OfType<IMethodSymbol>();

        return new BuilderNode(builder, implementations, methods);
    }

    private IEnumerable<(INamedTypeSymbol, bool)> GetBuilderDependencies(IMethodSymbol method)
    {
        foreach (IParameterSymbol parameter in method.Parameters)
        {
            if (TryGetBuilderType(parameter.Type, out INamedTypeSymbol? builder, out bool? isAction))
                yield return (builder!, isAction!.Value);
        }
    }

    private bool TryGetBuilderType(ITypeSymbol type, out INamedTypeSymbol? builder, out bool? isAction)
    {
        // Direct:
        // ITypeBuilder
        if (type is INamedTypeSymbol named &&
            _nodes.ContainsKey(named))
        {
            builder = named;
            isAction = false;
            return true;
        }

        // Action<IStatementBuilder>
        if (type is INamedTypeSymbol
            {
                Name: nameof(Action),
                TypeArguments.Length: 1
            } action &&
            action.TypeArguments[0] is INamedTypeSymbol argument &&
            _nodes.ContainsKey(argument))
        {
            builder = argument;
            isAction = true;
            return true;
        }

        builder = null;
        isAction = null;
        return false;
    }

    private static (string Prefix, string Subject) SplitMethodName(string name)
    {
        int index = 1;

        while (index < name.Length && !char.IsUpper(name[index]))
            index++;

        return (name[..index], name[index..]);
    }
}
