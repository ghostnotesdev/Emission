using Emission.Types;
using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public class MissingTypeDiagnostic : IDiagnostic
{
    private const string _messageBase = " is missing from the syntax tree. Either a namespace was changed or a package reference is missing.";
    public string Message { get; }
    public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MissingTypeError;

    public MissingTypeDiagnostic(string query, int arity)
    {
        Message = $"Type '{query}' with {arity} type arguments" + _messageBase;
    }

    public MissingTypeDiagnostic(string metadataName)
    {
        Message = $"Type '{metadataName}'" + _messageBase;
    }

    public MissingTypeDiagnostic(TypeQuery query)
    {
        Message = $"Type '{query}'" + _messageBase;
    }
}
