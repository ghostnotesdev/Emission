using Emission.Builders;
using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public class MissingConstructorDiagnostic(INamedTypeSymbol symbol, IEnumerable<IStatementParameterBuilder> mappings) : IDiagnostic
{
    public INamedTypeSymbol Symbol { get; } = symbol;
    public IEnumerable<IStatementParameterBuilder> Mappings { get; } = mappings;
    public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MissingConstructorError;
    public string Message => $"The type {Symbol.ToDisplayString()} doesn't have a publicly accessible constructor with the expected mappings: " +
        string.Join(", ", Mappings.Select(t => t.ParameterType?.Reference?.ToDisplayString() ?? "<unknown type>"));
}
