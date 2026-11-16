using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Generics;

public class GenericArgumentsBuilderTests
{
    [Fact]
    public void Validate_RejectsDuplicateNames()
    {
        var builder = new GenericArgumentsBuilder().WithArgument(a => a.UseName("T")).WithArgument(a => a.UseName("T"));

        Assert.Single(builder.Validate());
    }

    [Fact]
    public void BuildDeclaration_EmitsArguments()
    {
        var builder = new GenericArgumentsBuilder().WithArgument(a => a.UseName("T")).WithArgument(a => a.UseName("TValue"));

        Assert.Equal("<T, TValue>", TestSupport.CompilationTestHelpers.Build(builder.BuildDeclaration));
    }
}
