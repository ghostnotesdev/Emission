using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Property;

public class PropertyModifierBuilderTests
{
    [Fact]
    public void UsePropertyModifier_StoresModifier()
    {
        var builder = new PropertyModifierBuilder().UsePropertyModifier(PropertyModifier.Sealed);

        Assert.Equal(PropertyModifier.Sealed, builder.PropertyModifier);
    }

    [Fact]
    public void UsePropertyModifier_StoresCombinedModifiers()
    {
        var builder = new PropertyModifierBuilder().UsePropertyModifier(PropertyModifier.Virtual | PropertyModifier.Override);

        Assert.Equal(PropertyModifier.Virtual | PropertyModifier.Override, builder.PropertyModifier);
    }
}
