namespace Emission.Builders;
public interface IArrayBuilder : ICustomStatementBuilder
{
    ITypeBuilder EntryType { get; }
    IEnumerable<IStatementBuilder> Entries { get; }
    IArrayBuilder ConfigureType(Action<ITypeBuilder> configure);
    IArrayBuilder UseType(ITypeBuilder builder);
    IArrayBuilder UseEntries(IEnumerable<IStatementBuilder> entries);
    IArrayBuilder WithEntry(Action<IStatementBuilder> configure);
    IArrayBuilder WithEntry(IStatementBuilder builder);
}
