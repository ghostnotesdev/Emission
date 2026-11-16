using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Method;

public class ParameterBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingNameAndType()
    {
        Assert.Equal(2, new MethodParameterBuilder().Validate().Count());
    }

    [Fact]
    public void UseNameAndType_BuildsParameter()
    {
        var builder = new MethodParameterBuilder()
            .UseName("value Name")
            .ConfigureType(type => type.UseType(new TypeReference.Generated("string")));

        Assert.Equal("string value Name", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("string"));
        var builder = new MethodParameterBuilder().UseType(type);

        Assert.Same(type, builder.ParameterType);
    }
}
