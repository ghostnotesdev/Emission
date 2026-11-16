
namespace Emission.Builders;
public interface ISetterBuilder : IPropertyAccessorBuilder<ISetterBuilder>
{
    SetterAccessModifier Modifier { get; }
    ISetterBuilder UseAccessModifier(SetterAccessModifier modifier);
}
