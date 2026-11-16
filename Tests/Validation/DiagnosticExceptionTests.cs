using Emission.Validation;
using Xunit;

namespace Emission.UnitTests.Validation;

public class DiagnosticExceptionTests
{
    [Fact]
    public void Constructor_UsesDiagnosticMessageAndStoresDiagnostic()
    {
        var diagnostic = new MissingTypeDiagnostic("Missing", 0);
        var exception = new DiagnosticException(diagnostic);

        Assert.Same(diagnostic, exception.Diagnostic);
        Assert.Equal(diagnostic.Message, exception.Message);
    }
}
