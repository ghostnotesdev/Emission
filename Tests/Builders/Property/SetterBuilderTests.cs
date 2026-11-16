using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Property;

public class SetterBuilderTests
{
    [Fact]
    public void UseAccessModifier_BuildsPrivateSetter()
    {
        var builder = new SetterBuilder().UseAccessModifier(SetterAccessModifier.Private);

        Assert.Equal("private set;", TestSupport.CompilationTestHelpers.Build(sb => builder.Build(sb)));
    }

    [Fact]
    public void Validate_AllowsTheConfiguredAccessValue()
    {
        var builder = new SetterBuilder().UseAccessModifier(SetterAccessModifier.Internal);

        Assert.Empty(builder.Validate());
    }
}
