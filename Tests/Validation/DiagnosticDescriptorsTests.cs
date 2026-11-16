using Emission.Validation;
using Xunit;

namespace Emission.UnitTests.Validation;

public class DiagnosticDescriptorsTests
{
    [Fact]
    public void Id_ExposesStableIds()
    {
        Assert.Equal("E001", DiagnosticDescriptors.UnhandledError.Id);
        Assert.Equal("E003", DiagnosticDescriptors.InvalidBuilderConfigurationError.Id);
        Assert.Equal("E008", DiagnosticDescriptors.AmbiguousTypeError.Id);
    }
}
