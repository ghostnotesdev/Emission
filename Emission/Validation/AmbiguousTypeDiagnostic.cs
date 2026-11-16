using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public class AmbiguousTypeDiagnostic(string query, int arity, IEnumerable<INamedTypeSymbol> candidates) : IDiagnostic
{
    public string Query { get; } = query;
    public int Arity { get; } = arity;
    public IEnumerable<INamedTypeSymbol> Candidates { get; } = candidates;
    public string Message { get; } = $"Type '{query}' with {arity} type arguments is ambiguous between the following types: {string.Join(", ", candidates.Select(c => c.ToDisplayString()))}";
    public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AmbiguousTypeError;
}
