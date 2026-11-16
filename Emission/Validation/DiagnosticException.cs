namespace Emission.Validation;

public class DiagnosticException(IDiagnostic diagnostic) : Exception(diagnostic.Message)
{
    public IDiagnostic Diagnostic { get; } = diagnostic;
}

public class DiagnosticAggregateException(string message, IEnumerable<IDiagnostic> diagnostics) : AggregateException(message, diagnostics.Select(d => new DiagnosticException(d))) { }
