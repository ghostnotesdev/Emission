using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Base;

public class CompilationStringBuilderTests
{
    [Fact]
    public void AppendLine_AppendsIndentAtTheStartOfAline()
    {
        var builder = new CompilationStringBuilder(1);

        builder.AppendLine("value");

        Assert.Equal("\tvalue" + Environment.NewLine, builder.ToString());
    }

    [Fact]
    public void IncreaseIndentAndDecreaseIndent_ChangesIndentationWithoutGoingBelowZero()
    {
        var builder = new CompilationStringBuilder();

        builder.IncreaseIndent().AppendLine("one").DecreaseIndent().DecreaseIndent().AppendLine("two");

        Assert.Equal("\tone" + Environment.NewLine + "two" + Environment.NewLine, builder.ToString());
    }

    [Fact]
    public void AppendLinesWithSeparator_EmitsSeparatorsAndTrailingLine()
    {
        var builder = new CompilationStringBuilder();

        builder.AppendLinesWithSeparator(",", new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("a"))),
            new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("b"))));

        Assert.Equal("a," + Environment.NewLine + "b" + Environment.NewLine, builder.ToString());
    }

    [Fact]
    public void AppendLinesWithSeparator_AcceptsEnumerableCallbacks()
    {
        IEnumerable<IStatementBuilder> callbacks =
        [
            new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("a"))),
            new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("b")))
        ];
        var builder = new CompilationStringBuilder();

        builder.AppendLinesWithSeparator(",", callbacks);

        Assert.Equal("a," + Environment.NewLine + "b" + Environment.NewLine, builder.ToString());
    }

    [Fact]
    public void AppendWithSeparator_AcceptsEnumerableCallbacks()
    {
        IEnumerable<IStatementBuilder> callbacks =
        [
            new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("a"))),
            new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("b")))
        ];
        var builder = new CompilationStringBuilder();

        builder.AppendWithSeparator(",", callbacks);

        Assert.Equal("a,b", builder.ToString());
    }
}
