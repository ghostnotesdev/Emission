using Emission.Validation;
using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace Emission.Types;

public readonly struct NamespaceLookupResult(string _namespace, INamespaceSymbol? result, IEnumerable<IDiagnostic>? diagnostics)
{
    public INamespaceSymbol? Result { get; } = result;
    public IEnumerable<IDiagnostic> Diagnostics { get; } = diagnostics ?? [];
    public DiagnosticAggregateException? Exception { get; } = result is null ? new($"Namespace '{_namespace}' was not found in the current compilation", diagnostics ?? []) : null;

    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool Success => Result is not null;
}
