using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class ArrayBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingEntryType()
    {
        Assert.Single(new ArrayBuilder().Validate());
    }

    [Fact]
    public void UseEntries_BuildsArrayInitializer()
    {
        var builder = new ArrayBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("int")))
            .UseEntries([
                new StatementBuilder().UseStatement("1"),
                new StatementBuilder().UseStatement("2")]);

        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Contains("new int[]", output);
        Assert.Contains("1,", output);
        Assert.Contains("2", output);
    }

    [Fact]
    public void UseEntries_AcceptsEnumerable()
    {
        IEnumerable<IStatementBuilder> entries =
            new[] { "1", "2" }.Select(value => (IStatementBuilder)new StatementBuilder().UseStatement(value));

        var builder = new ArrayBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("int")))
            .UseEntries(entries);

        Assert.Equal(2, builder.Entries.Count());
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("int"));
        var builder = new ArrayBuilder().UseType(type);

        Assert.Same(type, builder.EntryType);
    }
}
