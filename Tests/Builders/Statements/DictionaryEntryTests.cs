using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class DictionaryEntryTests
{
    [Fact]
    public void Constructor_StoresKeyAndValueBuilders()
    {
        var key = new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("key")));
        var value = new StatementBuilder().UseStatement(new CompilationExpression(sb => sb.Append("value")));
        var entry = new DictionaryEntry(key, value);

        Assert.Same(key, entry.Key);
        Assert.Same(value, entry.Value);
    }

    [Fact]
    public void Constructor_ActionsConfigureKeyAndValueBuilders()
    {
        var entry = new DictionaryEntry(
            key => key.UseStatement("key"),
            value => value.UseStatement("value"));

        Assert.Equal("key", TestSupport.CompilationTestHelpers.Build(entry.Key.Build));
        Assert.Equal("value", TestSupport.CompilationTestHelpers.Build(entry.Value.Build));
    }
}
