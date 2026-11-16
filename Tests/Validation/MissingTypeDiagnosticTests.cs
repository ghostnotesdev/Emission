using Emission.Types;
using Emission.Validation;
using Xunit;

namespace Emission.UnitTests.Validation;

public class MissingTypeDiagnosticTests
{
    [Fact]
    public void Constructor_CreateExpectedMessages()
    {
        Assert.Contains("Missing", new MissingTypeDiagnostic("Missing", 1).Message);
        Assert.Contains("System.String", new MissingTypeDiagnostic("System.String").Message);
        Assert.Contains("List`1", new MissingTypeDiagnostic(TypeQuery.FromName("List", TypeQuery.FromName("T"))).Message);
    }
}
