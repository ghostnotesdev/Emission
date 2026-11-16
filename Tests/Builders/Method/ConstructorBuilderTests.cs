using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Method;

public class ConstructorBuilderTests
{
    [Fact]
    public void ConfigureBodyExpression_BuildsConstructorWithParameter()
    {
        var builder = new ConstructorBuilder()
            .WithParameter(parameter => parameter.UseName("name").ConfigureType(type => type.UseType(new TypeReference.Generated("string"))))
            .ConfigureBodyExpression(body => body
                .WithStatement(sb => sb.UseStatement("this.Name = name;")));

        var diags = builder.Validate().ToList();
        var sb = new CompilationStringBuilder();
        builder.Build(sb, "Person");
        var output = sb.ToString();

        Assert.Contains("Person(string name)", output);
        Assert.Contains("this.Name = name;", output);
    }

    [Fact]
    public void Validate_ReportsInvalidParameter()
    {
        var builder = new ConstructorBuilder()
            .ConfigureBodyExpression(body => body
                .WithStatement(sb => sb.UseStatement("{}")))
            .WithParameter(_ => { });

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("UseType"));
    }
}
