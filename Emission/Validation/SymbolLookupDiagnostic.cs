using Emission.Types;
using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public class SymbolLookupDiagnostic : IDiagnostic
{
    private const string _messageBase = "Symbol lookup for type '{0}' failed: {1}";
    public string Message { get; }
    public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SymbolLookupError;

    public SymbolLookupDiagnostic(TypeQuery query, string error)
    {
        Message = string.Format(_messageBase, query, error);
    }
}
