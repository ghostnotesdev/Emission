using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
public abstract class BuilderDiagnosticBase(IBuilder builder) : IDiagnostic
{
    public IBuilder OriginatingBuilder => builder;
    public abstract string Message { get; }
    public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.InvalidBuilderConfigurationError;

    protected string GetBuilderKey(IBuilder builder) => string.IsNullOrEmpty(builder.DiagnosticName)
        ? builder.GetType().Name
        : builder.GetType().Name + ": " + builder.DiagnosticName;
}
