using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Method;

public class ConstructorModifierBuilderTests
{
    [Fact]
    public void Build_EmitsPublicConstructorModifier()
    {
        var builder = new ConstructorModifierBuilder().UseAccessModifier(AccessModifier.Public);

        Assert.Equal("public", TestSupport.CompilationTestHelpers.Build(builder.Build));
        Assert.Empty(builder.Validate());
    }
}
