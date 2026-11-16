using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Method;

public class MethodBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingNameAndImplementation()
    {
        Assert.Equal(2, new MethodBuilder().Validate().Count());
    }

    [Fact]
    public void UseNameAndBody_BuildsPascalCaseMethod()
    {
        var builder = new MethodBuilder()
            .UseName("GetValue")
            .ConfigureReturnType(type => type.UseType(new TypeReference.Generated("string")))
            .ConfigureBodyExpression(body => body
                .WithStatement(sb => sb.UseStatement("return value;")));

        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildImplementation);

        Assert.Contains("string GetValue()", output);
        Assert.Contains("return value;", output);
    }

    [Fact]
    public void AsInterfaceMethod_RequiresPublicAccess()
    {
        var builder = new MethodBuilder().UseName("run").AsInterfaceMethod();

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("public"));
    }

    [Fact]
    public void Validate_ReportsInvalidParameter()
    {
        var builder = new MethodBuilder()
            .UseName("run")
            .ConfigureBodyExpression(body => body
                .WithStatement(sb => sb.UseStatement("return;")))
            .WithParameter(_ => { });

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("UseType"));
    }

    [Fact]
    public void ConfigureExpression_ReplacesBody()
    {
        var builder = new MethodBuilder().UseName("run")
            .ConfigureBodyExpression(body => body
                .WithStatement(sb => sb.UseStatement("return;")))
            .ConfigureExpression(expression => expression
                .UseExpression(new StatementBuilder().UseStatement("value")));

        Assert.NotNull(builder.Expression);
        Assert.Null(builder.Body);
    }
}
