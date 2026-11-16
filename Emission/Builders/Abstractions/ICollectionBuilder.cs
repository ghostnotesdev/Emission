
namespace Emission.Builders;
public interface ICollectionBuilder : ICustomStatementBuilder
{
    ITypeBuilder CollectionType { get; }
    ITypeBuilder EntryType { get; }
    IEnumerable<IStatementBuilder> Entries { get; }

    ICollectionBuilder ConfigureCollectionType(Action<ITypeBuilder> configure);
    ICollectionBuilder UseCollectionType(ITypeBuilder builder);
    ICollectionBuilder ConfigureEntryType(Action<ITypeBuilder> configure);
    ICollectionBuilder UseEntryType(ITypeBuilder builder);
    ICollectionBuilder UseEntries(IEnumerable<IStatementBuilder> entries);
    ICollectionBuilder WithEntry(Action<IStatementBuilder> configure);
    ICollectionBuilder WithEntry(IStatementBuilder builder);
}
