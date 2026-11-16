using Emission.Builders;
using Moq;
using Xunit;

namespace Emission.UnitTests.Builders.Validation;

public class BuilderDiagnosticTests
{
    [Fact]
    public void WithParent_AddsBuilderLocationToMessage()
    {
        var originating = new Mock<IBuilder>();
        originating.SetupGet(builder => builder.DiagnosticName).Returns("child");
        var parent = new Mock<IBuilder>();
        parent.SetupGet(builder => builder.DiagnosticName).Returns("parent");
        var diagnostic = new Mock<Emission.Validation.IDiagnostic>();
        diagnostic.SetupGet(value => value.Message).Returns("invalid");
        var builderDiagnostic = new BuilderDiagnostic(originating.Object, diagnostic.Object).WithParent(parent.Object);

        Assert.Contains("invalid", builderDiagnostic.Message);
        Assert.Contains("Location:", builderDiagnostic.Message);
        Assert.Contains("IBuilderProxy: parent", builderDiagnostic.Message);
        Assert.Contains("-> IBuilderProxy: child", builderDiagnostic.Message);
    }
}
