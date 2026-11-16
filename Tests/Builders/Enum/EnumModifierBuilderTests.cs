using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Enum;

public class EnumModifierBuilderTests
{
    [Fact]
    public void UseAccessModifier_BuildsModifier()
    {
        var builder = new EnumModifierBuilder().UseAccessModifier(AccessModifier.Public);

        Assert.Equal("public", TestSupport.CompilationTestHelpers.Build(builder.Build));
        Assert.Empty(builder.Validate());
    }
}
