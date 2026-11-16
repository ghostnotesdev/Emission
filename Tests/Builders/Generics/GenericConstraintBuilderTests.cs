using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Generics;

public class GenericConstraintBuilderTests
{
    [Fact]
    public void Validate_RejectsStructWithConstructorConstraint()
    {
        var builder = new GenericConstraintBuilder().UseConstraint(GenericPrimaryConstraint.Struct).AsParameterlessConstructor();

        Assert.Single(builder.Validate());
    }

    [Fact]
    public void Build_EmitsPrimaryTypeAndConstructorConstraint()
    {
        var builder = new GenericConstraintBuilder().UseConstraint(GenericPrimaryConstraint.Class).AsParameterlessConstructor();

        Assert.Equal("class, new()", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void Inherits_TypeConstraintIsReportedAsPresent()
    {
        var builder = new GenericConstraintBuilder()
            .WithImplementation(type => type.UseType(new TypeReference.Generated("BaseType")));

        Assert.True(builder.HasConstraints);
        Assert.Equal("BaseType", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
