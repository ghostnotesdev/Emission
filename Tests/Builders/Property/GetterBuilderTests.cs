using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Property;

public class GetterBuilderTests
{
    [Fact]
    public void UseAutoAccessor_BuildsAutoGetter()
    {
        var builder = new GetterBuilder();

        Assert.Equal("get;", TestSupport.CompilationTestHelpers.Build(sb => builder.Build(sb)));
    }

    [Fact]
    public void ConfigureExpressionAccessor_BuildsExpressionGetter()
    {
        var builder = new GetterBuilder()
            .ConfigureExpressionAccessor(expression => expression
                .UseExpression(new StatementBuilder().UseStatement("value")));

        Assert.Equal("get => value;", TestSupport.CompilationTestHelpers.Build(sb => builder.Build(sb)));
    }

    [Fact]
    public void ConfigureExpressionAccessor_BuildsExpressionGetterWithoutAccessor()
    {
        var builder = new GetterBuilder()
            .ConfigureExpressionAccessor(expression => expression
                .UseExpression(new StatementBuilder().UseStatement("value")));

        Assert.Equal("=> value;", TestSupport.CompilationTestHelpers.Build(sb => builder.Build(sb, false)));
    }
}
