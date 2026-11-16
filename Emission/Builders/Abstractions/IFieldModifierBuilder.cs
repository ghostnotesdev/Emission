
namespace Emission.Builders;
public interface IFieldModifierBuilder : IModifierBuilder<IFieldModifierBuilder>
{
    FieldModifier FieldModifier { get; }
    IFieldModifierBuilder UseFieldModifier(FieldModifier modifier);
}
