
namespace Emission.Builders;
public interface IModifierBuilder<TInterface> : IBuilder where TInterface : IModifierBuilder<TInterface>
{
    AccessModifier AccessModifier { get; }
    TInterface UseAccessModifier(AccessModifier modifier);
    void Build(ICompilationStringBuilder icsb);
}
