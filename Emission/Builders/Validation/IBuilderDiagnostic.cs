using Emission.Validation;

namespace Emission.Builders;
public interface IBuilderDiagnostic<TBuilderDiagnostic> : IDiagnostic where TBuilderDiagnostic : IDiagnostic
{
    IBuilder OriginatingBuilder { get; }
    TBuilderDiagnostic WithParent(IBuilder builder);
    IDiagnostic OriginatingDiagnostic { get; }
}
