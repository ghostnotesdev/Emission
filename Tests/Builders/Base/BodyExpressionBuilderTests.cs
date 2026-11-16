using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Base;

public class BodyExpressionBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingBody()
    {
        Assert.Single(new BodyExpressionBuilder().Validate());
    }

    [Fact]
    public void UseBody_StringBuildsBlock()
    {
        var builder = new BodyExpressionBuilder()
            .WithStatement(body => body.UseStatement("return value;"));

        var csb = new CompilationStringBuilder();
        builder.Build(csb);
        var output = csb.ToString();

        Assert.Equal("{" + Environment.NewLine + "\treturn value;" + Environment.NewLine + "}", output);
    }

    [Fact]
    public void UseBody_StringBuildsBlock_NewLine()
    {
        var builder = new BodyExpressionBuilder()
            .WithStatement(body => body.UseStatement("return value;"));

        var csb = new CompilationStringBuilder();
        builder.Build(csb, true);
        var output = csb.ToString();

        Assert.Equal("{" + Environment.NewLine + "\treturn value;" + Environment.NewLine + "}" + Environment.NewLine, output);
    }
}
