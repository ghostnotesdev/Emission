using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Base;

public class ExpressionBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingExpression()
    {
        Assert.Single(new ExpressionBuilder().Validate());
    }

    [Fact]
    public void UseExpression_StringBuildsExpressionWithTerminator()
    {
        var builder = new ExpressionBuilder()
            .UseExpression(new StatementBuilder().UseStatement("value"));

        Assert.Equal("=> value;", TestSupport.CompilationTestHelpers.Build(sb => builder.Build(sb)));
    }
}
