using Emission.Extensions;
using Xunit;

namespace Emission.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ToCamelCase_ChangesOnlyTheFirstCharacter()
    {
        Assert.Equal("value", "Value".ToCamelCase());
        Assert.Equal("a", "A".ToCamelCase());
    }

    [Fact]
    public void ToPascalCase_RemovesSeparatorsAndCapitalizesWords()
    {
        Assert.Equal("HelloWorld", " hello-world ".ToPascalCase());
        Assert.Equal(string.Empty, "   ".ToPascalCase());
    }
}
