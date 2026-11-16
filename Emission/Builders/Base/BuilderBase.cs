using Emission.Validation;

namespace Emission.Builders;public abstract class BuilderBase : IBuilder
{
    public IDiagnostic PropagateDiagnostic(IDiagnostic diagnostic)
    {
        if (diagnostic is BuilderDiagnostic builderDiagnostic)
        {
            return builderDiagnostic.WithParent(this);
        }

        if (diagnostic is BuilderDiagnosticBase origin)
        {
            return new BuilderDiagnostic(origin.OriginatingBuilder, diagnostic)
                .WithParent(this);
        }

        return new BuilderDiagnostic(this, diagnostic);
    }

    public IEnumerable<IDiagnostic> PropagateDiagnostics(IEnumerable<IDiagnostic> diagnostics)
    {
        return diagnostics.Select(PropagateDiagnostic);
    }

    public virtual string DiagnosticName { get; } = string.Empty;
    public abstract IEnumerable<IDiagnostic> Validate();
}
