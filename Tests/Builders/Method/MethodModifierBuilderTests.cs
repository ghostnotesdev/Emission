using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Method;

public class MethodModifierBuilderTests
{
    [Fact]
    public void Validate_RejectsVirtualAndOverride()
    {
        var builder = new MethodModifierBuilder().UseMethodModifier(MethodModifier.Virtual | MethodModifier.Override);

        Assert.Single(builder.Validate());
    }

    [Fact]
    public void Build_EmitsAccessAndMethodModifiers()
    {
        var builder = new MethodModifierBuilder()
            .UseAccessModifier(AccessModifier.Public)
            .UseMethodModifier(MethodModifier.Static | MethodModifier.Async);

        Assert.Equal("public static async", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
