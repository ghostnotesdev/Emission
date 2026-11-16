
namespace Emission.Builders;
public interface IInterfaceModifierBuilder : IModifierBuilder<IInterfaceModifierBuilder>
{
    InterfaceModifier InterfaceModifier { get; }
    IInterfaceModifierBuilder UseInterfaceModifier(InterfaceModifier modifier);
}
