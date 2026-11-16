using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Generics;

public class GenericArgumentBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingName()
    {
        Assert.Single(new GenericArgumentBuilder().Validate());
    }

    [Fact]
    public void BuildConstraints_EmitsConstraintClause()
    {
        var builder = new GenericArgumentBuilder().UseName("T").ConfigureConstraints(constraints => constraints
            .UseConstraint(GenericPrimaryConstraint.Class)
            .WithImplementation(type => type.UseType(new TypeReference.Generated("Base")))
            .AsParameterlessConstructor());

        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildConstraints);

        Assert.Equal("where T : class, Base, new()", output);
    }
}
