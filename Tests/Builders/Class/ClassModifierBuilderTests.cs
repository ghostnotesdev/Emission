using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Class;

public class ClassModifierBuilderTests
{
    [Fact]
    public void Validate_RejectsMultipleDeclarationModifiers()
    {
        var builder = new ClassModifierBuilder().UseClassModifier(ClassModifier.Abstract | ClassModifier.Sealed);

        Assert.Single(builder.Validate());
    }

    [Fact]
    public void UseClassModifier_BuildsClassModifiers()
    {
        var builder = new ClassModifierBuilder().UseAccessModifier(AccessModifier.Public).UseClassModifier(ClassModifier.Sealed);

        Assert.Equal("public sealed", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
