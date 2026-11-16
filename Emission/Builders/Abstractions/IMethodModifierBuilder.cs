
namespace Emission.Builders;
public interface IMethodModifierBuilder : IModifierBuilder<IMethodModifierBuilder>
{
    MethodModifier MethodModifier { get; }
    IMethodModifierBuilder UseMethodModifier(MethodModifier modifier);
}
