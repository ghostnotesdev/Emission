using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class StatementBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingStatement()
    {
        Assert.Single(new StatementBuilder().Validate());
    }

    [Fact]
    public void UseStatement_StringBuildsLiteral()
    {
        var builder = new StatementBuilder().UseStatement("value");

        Assert.Equal("value", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseStatement_CompilationExpressionBuildsDeferredOutput()
    {
        var builder = new StatementBuilder()
            .UseStatement(new CompilationExpression(sb => sb.Append("value")));

        Assert.Equal("value", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseStatement_CustomBuilderBuildsNestedOutput()
    {
        var custom = new ArrayBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("int")))
            .WithEntry(new StatementBuilder().UseStatement("1"));
        var builder = new StatementBuilder().UseStatement(custom);

        Assert.Contains("new int[]", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void ConfigureStatement_PassesConcreteBuilderToCallback()
    {
        var builder = new StatementBuilder()
            .ConfigureStatement<ArrayBuilder>(array => array
                .ConfigureType(type => type.UseType(new TypeReference.Generated("int")))
                .WithEntry(new StatementBuilder().UseStatement("1")));

        Assert.Contains("new int[]", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseStatement_ReplacesPreviousStatement()
    {
        var builder = new StatementBuilder()
            .UseStatement("first")
            .UseStatement(new CompilationExpression(sb => sb.Append("second")));

        Assert.Equal("second", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
