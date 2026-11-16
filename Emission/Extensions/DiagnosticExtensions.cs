using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Extensions;

public static class DiagnosticExtensions
{
    public static Diagnostic ToDiagnostic(this IDiagnostic diagnostic) => Diagnostic.Create(diagnostic.Descriptor, null, diagnostic.Message);
}
