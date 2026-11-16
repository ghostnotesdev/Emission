using Emission.Builders;
using Moq;
using Xunit;

namespace Emission.UnitTests.Builders.Validation;

public class InvalidBuilderConfigurationDiagnosticTests
{
    [Fact]
    public void Constructor_StoresOriginatingBuilderAndMessage()
    {
        var builder = new Mock<IBuilder>().Object;
        var diagnostic = new InvalidBuilderConfigurationDiagnostic(builder, "bad configuration");

        Assert.Same(builder, diagnostic.OriginatingBuilder);
        Assert.Equal("bad configuration", diagnostic.Message);
    }
}
