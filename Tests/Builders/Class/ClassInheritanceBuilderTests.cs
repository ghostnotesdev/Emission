using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Class;

public class ClassInheritanceBuilderTests
{
    [Fact]
    public void Build_EmitsBaseAndInterfaces()
    {
        var builder = new ClassInheritanceBuilder()
            .ConfigureBaseClass(type => type.UseType(new TypeReference.Generated("Base")))
            .WithImplementation(type => type.UseType(new TypeReference.Generated("IFirst")))
            .WithImplementation(type => type.UseType(new TypeReference.Generated("ISecond")));

        Assert.Equal(": Base, IFirst, ISecond", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void WithImplementation_StoresConfiguredType()
    {
        var builder = new ClassInheritanceBuilder().WithImplementation(type => type.UseType(new TypeReference.Generated("Base")));

        Assert.Single(builder.Types);
    }
}
