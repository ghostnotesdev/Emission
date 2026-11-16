using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Base;

public class TypeBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingType()
    {
        Assert.Single(new TypeBuilder().Validate());
    }

    [Fact]
    public void UseType_BuildsTheTypeToken()
    {
        var builder = new TypeBuilder().UseType(new TypeReference.Generated("global::System.String"));

        Assert.Equal("global::System.String", TestSupport.CompilationTestHelpers.Build(builder.BuildType));
    }

    [Fact]
    public void UseType_ResolutionResult_PreservesFailureDiagnostics()
    {
        var result = new TypeResolutionResult(
            TypeQuery.FromName("Missing"),
            null,
            [new Emission.Validation.MissingTypeDiagnostic(TypeQuery.FromName("Missing"))]);

        var diagnostics = new TypeBuilder().UseType(result).Validate().ToArray();

        Assert.NotEmpty(diagnostics);
        Assert.Contains("Missing", diagnostics[0].Message);
    }
}
