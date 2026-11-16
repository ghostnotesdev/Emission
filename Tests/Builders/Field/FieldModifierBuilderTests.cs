using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Field;

public class FieldModifierBuilderTests
{
    [Fact]
    public void Validate_RejectsConstCombinedWithAnotherModifier()
    {
        var builder = new FieldModifierBuilder().UseFieldModifier(FieldModifier.Const | FieldModifier.Static);

        Assert.Single(builder.Validate());
    }

    [Fact]
    public void Validate_RejectsReadonlyAndVolatile()
    {
        var builder = new FieldModifierBuilder().UseFieldModifier(FieldModifier.ReadOnly | FieldModifier.Volatile);

        Assert.Single(builder.Validate());
    }
}
