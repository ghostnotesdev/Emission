using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Types;

public class TypeResolutionResultTests
{
    [Fact]
    public void Constructor_StoresResultAndDiagnostics()
    {
        var query = TypeQuery.FromName("Resolved");
        var type = new TypeReference.Generated("string");
        var diagnostics = new[] { new Emission.Validation.MissingTypeDiagnostic("Missing", 0) };

        var result = new TypeResolutionResult(query, type, diagnostics);

        Assert.Same(type, result.Result);
        Assert.Same(diagnostics, result.Diagnostics);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Constructor_ExposesExceptionWhenResultIsMissing()
    {
        var query = TypeQuery.FromName("Missing");
        var diagnostic = new Emission.Validation.MissingTypeDiagnostic("Missing", 0);

        var result = new TypeResolutionResult(query, null, [diagnostic]);

        var exception = Assert.IsType<Emission.Validation.DiagnosticAggregateException>(result.Exception);
        Assert.Contains("Missing", exception.Message);
        var innerException = Assert.IsType<Emission.Validation.DiagnosticException>(Assert.Single(exception.InnerExceptions));
        Assert.Same(diagnostic, innerException.Diagnostic);
    }
}
