using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public class NamespaceLookupDiagnostic : IDiagnostic
{
    private const string _messageBase = "Namespace lookup for namespace '{0}' failed: {1}";
    public string Message { get; }
    public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.NamespaceLookupError;

    public NamespaceLookupDiagnostic(string _namespace, string error)
    {
        Message = string.Format(_messageBase, _namespace, error);
    }
}
