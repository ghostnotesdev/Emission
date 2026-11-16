using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Interface;

public class InterfaceInheritanceBuilderTests
{
    [Fact]
    public void Extends_BuildsAllBaseInterfaces()
    {
        var builder = new InterfaceInheritanceBuilder()
            .WithImplementation(type => type.UseType(new TypeReference.Generated("IFirst")))
            .WithImplementation(type => type.UseType(new TypeReference.Generated("ISecond")));

        Assert.Equal(": IFirst, ISecond", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
