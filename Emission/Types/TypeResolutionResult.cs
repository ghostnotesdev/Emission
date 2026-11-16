using Emission.Validation;
using System.Diagnostics.CodeAnalysis;

namespace Emission.Types;

public readonly struct TypeResolutionResult(TypeQuery query, TypeReference? result, IEnumerable<IDiagnostic>? diagnostics = null)
{
    public TypeReference? Result { get; } = result;
    public IEnumerable<IDiagnostic> Diagnostics { get; } = diagnostics ?? [];
    public DiagnosticAggregateException? Exception { get; } = result is null ? new($"Type '{query}' is not available in the current compilation", diagnostics ?? []) : null;

    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool Success => Result is not null;
}
