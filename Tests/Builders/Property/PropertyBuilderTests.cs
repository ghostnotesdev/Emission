using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Property;

public class PropertyBuilderTests
{
    [Fact]
    public void PropertyName_ReportsDefaultValue()
    {
        var builder = new PropertyBuilder();

        Assert.Equal(string.Empty, builder.PropertyName);
        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("PropertyName"));
    }

    [Fact]
    public void ConfigureAssignment_BuildsAutoProperty()
    {
        var builder = new PropertyBuilder()
            .UseName("DisplayName")
            .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
            .ConfigureAssignment(assignment => assignment
                .UseAssignment(new StatementBuilder().UseStatement("\"default\"")));

        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildImplementation);

        Assert.Contains("string DisplayName { get; set; } = \"default\";", output);
    }

    [Fact]
    public void AsInterfaceProperty_RequiresPublicAccess()
    {
        var builder = new PropertyBuilder().UseName("value").AsInterfaceProperty();

        Assert.True(builder.IsInterfaceProperty);
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("string"));
        var builder = new PropertyBuilder().UseType(type);

        Assert.Same(type, builder.PropertyType);
    }
}
