namespace Emission.Builders;
public interface IEnumValueBuilder : ICustomStatementBuilder
{
    ITypeBuilder EnumType { get; }
    string? RequestedValue { get; }
    IEnumValueBuilder ConfigureType(Action<ITypeBuilder> configure);
    IEnumValueBuilder UseType(ITypeBuilder builder);
    IEnumValueBuilder UseValue(string value);
}
