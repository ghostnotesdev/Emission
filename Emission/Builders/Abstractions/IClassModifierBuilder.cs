
namespace Emission.Builders;
public interface IClassModifierBuilder : IModifierBuilder<IClassModifierBuilder>
{
    ClassModifier ClassModifier { get; }
    IClassModifierBuilder UseClassModifier(ClassModifier modifier);
}
