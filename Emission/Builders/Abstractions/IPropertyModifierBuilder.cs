
namespace Emission.Builders;
public interface IPropertyModifierBuilder : IModifierBuilder<IPropertyModifierBuilder>
{
    PropertyModifier PropertyModifier { get; }
    IPropertyModifierBuilder UsePropertyModifier(PropertyModifier modifier);
}
