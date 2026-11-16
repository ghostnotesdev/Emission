using Emission.Validation;

namespace Emission.Builders;
public interface IBuilder
{
    string DiagnosticName { get; }
    IEnumerable<IDiagnostic> Validate();
}
