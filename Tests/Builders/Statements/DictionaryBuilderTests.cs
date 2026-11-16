using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class DictionaryBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingDictionaryType()
    {
        Assert.Single(new DictionaryBuilder().Validate());
    }

    [Fact]
    public void UseEntries_BuildsDictionaryInitializer()
    {
        var builder = new DictionaryBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("Dictionary<string, int>")))
            .WithEntry(
                new DictionaryEntry(
                    new StatementBuilder().UseStatement("\"one\""),
                    new StatementBuilder().UseStatement("1")))
            .WithEntry(new DictionaryEntry(
                    new StatementBuilder().UseStatement("\"two\""),
                    new StatementBuilder().UseStatement("2")));

        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Contains("new Dictionary<string, int>()", output);
        Assert.Contains("{ \"one\", 1 },", output);
        Assert.Contains("{ \"two\", 2 }", output);
    }

    [Fact]
    public void UseEntries_AcceptsEnumerable()
    {
        IEnumerable<DictionaryEntry> entries =
        [
            new DictionaryEntry(
                new StatementBuilder().UseStatement("\"one\""),
                new StatementBuilder().UseStatement("1")),
            new DictionaryEntry(
                new StatementBuilder().UseStatement("\"two\""),
                new StatementBuilder().UseStatement("2"))
        ];

        var builder = new DictionaryBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("Dictionary<string, int>")))
            .UseEntries(entries);

        Assert.Equal(2, builder.Entries.Count());
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("Dictionary<string, int>"));
        var builder = new DictionaryBuilder().UseType(type);

        Assert.Same(type, builder.DictionaryType);
    }
}
