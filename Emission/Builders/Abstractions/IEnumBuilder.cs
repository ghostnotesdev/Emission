namespace Emission.Builders;
public interface IEnumBuilder : ITopLevelBuilder<IEnumBuilder>, ITypeBuilder
{
    IEnumModifierBuilder ModifierBuilder { get; }
    bool IsFlags { get; }
    IEnumerable<string> Values { get; }

    IEnumBuilder AsFlags();
    IEnumBuilder ConfigureModifier(Action<IEnumModifierBuilder> configure);
    IEnumBuilder UseValues(IEnumerable<string> values);
    IEnumBuilder WithValue(string value);
    void BuildEnum(ICompilationStringBuilder icsb);
}
