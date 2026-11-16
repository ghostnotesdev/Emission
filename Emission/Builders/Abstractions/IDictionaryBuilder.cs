
namespace Emission.Builders;
public interface IDictionaryBuilder : ICustomStatementBuilder
{
    ITypeBuilder DictionaryType { get; }
    IEnumerable<DictionaryEntry> Entries { get; }
    IDictionaryBuilder ConfigureType(Action<ITypeBuilder> configure);
    IDictionaryBuilder UseType(ITypeBuilder builder);
    IDictionaryBuilder UseEntries(IEnumerable<DictionaryEntry> entries);
    IDictionaryBuilder WithEntry(DictionaryEntry entry);
}
