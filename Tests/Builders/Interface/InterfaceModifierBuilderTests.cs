using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Interface;

public class InterfaceModifierBuilderTests
{
    [Fact]
    public void UseInterfaceModifier_BuildsUnsafeModifier()
    {
        var builder = new InterfaceModifierBuilder()
            .UseAccessModifier(AccessModifier.Public)
            .UseInterfaceModifier(InterfaceModifier.Unsafe);

        Assert.Equal("public unsafe", TestSupport.CompilationTestHelpers.Build(builder.Build));
        Assert.Empty(builder.Validate());
    }
}
