using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class CollectionBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingCollectionAndEntryTypes()
    {
        Assert.Equal(2, new CollectionBuilder().Validate().Count());
    }

    [Fact]
    public void UseTypesAndEntries_BuildsCollectionInitializer()
    {
        var builder = new CollectionBuilder()
            .ConfigureCollectionType(type => type.UseType(new TypeReference.Generated("List<int>")))
            .ConfigureEntryType(type => type.UseType(new TypeReference.Generated("int")))
            .UseEntries([
                new StatementBuilder().UseStatement("1"),
                new StatementBuilder().UseStatement("2")]);

        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Contains("new List<int>(new int[]", output);
        Assert.Contains("1,", output);
    }

    [Fact]
    public void UseEntries_AcceptsEnumerable()
    {
        IEnumerable<IStatementBuilder> entries =
            new[] { "1", "2" }.Select(value => (IStatementBuilder)new StatementBuilder().UseStatement(value));

        var builder = new CollectionBuilder().UseEntries(entries);

        Assert.Equal(2, builder.Entries.Count());
    }

    [Fact]
    public void UseCollectionType_StoresSharedTypeBuilder()
    {
        var collectionType = new TypeBuilder()
            .UseType(new TypeReference.Generated("List<int>"));
        var builder = new CollectionBuilder().UseCollectionType(collectionType);

        Assert.Same(collectionType, builder.CollectionType);
    }

    [Fact]
    public void UseEntryType_StoresSharedTypeBuilder()
    {
        var entryType = new TypeBuilder()
            .UseType(new TypeReference.Generated("int"));
        var builder = new CollectionBuilder().UseEntryType(entryType);

        Assert.Same(entryType, builder.EntryType);
    }

    [Fact]
    public void UseTypesAndEntries_BuildsEmptyCollectionInitializer()
    {
        var builder = new CollectionBuilder()
            .UseCollectionType(new TypeBuilder()
                .UseType(new TypeReference.Generated("List<int>")))
            .UseEntryType(new TypeBuilder()
                .UseType(new TypeReference.Generated("int")));

        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Equal(
            "new List<int>(new int[]" + Environment.NewLine +
            "{" + Environment.NewLine +
            "})",
            output);
    }

    [Fact]
    public void UseTypesAndEntries_EmitsEntriesWithSeparators()
    {
        var builder = new CollectionBuilder()
            .UseCollectionType(new TypeBuilder()
                .UseType(new TypeReference.Generated("List<int>")))
            .UseEntryType(new TypeBuilder()
                .UseType(new TypeReference.Generated("int")))
            .UseEntries([
                new StatementBuilder().UseStatement("1"),
                new StatementBuilder().UseStatement("2")]);

        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Equal(
            "new List<int>(new int[]" + Environment.NewLine +
            "{" + Environment.NewLine +
            "\t1," + Environment.NewLine +
            "\t2" + Environment.NewLine +
            "})",
            output);
    }
}
