using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public interface IDiagnostic
{
    string Message { get; }
    DiagnosticDescriptor Descriptor { get; }
}
