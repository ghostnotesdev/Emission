using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Field;

public class FieldBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingNameAndConfiguration()
    {
        Assert.Equal(2, new FieldBuilder().Validate().Count());
    }

    [Fact]
    public void UseNameAndConfiguration_BuildsField()
    {
        var builder = new FieldBuilder()
            .UseName("FieldName")
            .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
            .ConfigureAssignment(assignment => assignment
                .UseAssignment(new StatementBuilder().UseStatement("\"value\"")));

        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Equal("string FieldName = \"value\";" + Environment.NewLine, output);
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("string"));
        var builder = new FieldBuilder();
        builder.UseType(type);

        Assert.Same(type, builder.TypeBuilder);
    }
}
