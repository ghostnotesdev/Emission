using Emission.Validation;
using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace Emission.Types;

public readonly struct SymbolLookupResult(TypeQuery query, ITypeSymbol? result, IEnumerable<IDiagnostic>? diagnostics)
{
    public ITypeSymbol? Result { get; } = result;
    public IEnumerable<IDiagnostic> Diagnostics { get; } = diagnostics ?? [];
    public DiagnosticAggregateException? Exception { get; } = result is null ? new($"Type '{query}' is not available in the current compilation", diagnostics ?? []) : null;
    public INamedTypeSymbol? NamedSymbol => Success ? ParseNamedSymbol(Result) : null;

    [MemberNotNullWhen(true, nameof(Result), nameof(NamedSymbol))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool Success => Result is not null;

    private INamedTypeSymbol ParseNamedSymbol(ITypeSymbol symbol)
    {
        return symbol switch
        {
            INamedTypeSymbol named => named,
            IArrayTypeSymbol array => ParseNamedSymbol(array.ElementType),
            _ => throw new InvalidOperationException("Symbol was not a supported type: " + symbol)
        };
    }
}
