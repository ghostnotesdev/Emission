using Emission.Validation;

namespace Emission.Builders;
public class InterfaceModifierBuilder : ModifierBuilder<InterfaceModifierBuilder, IInterfaceModifierBuilder>, IInterfaceModifierBuilder
{
    private readonly SimpleBuilderProperty<InterfaceModifierBuilder, InterfaceModifier> _interfaceModifier;
    protected override Modifier Modifier => (Modifier)InterfaceModifier;
    public InterfaceModifier InterfaceModifier => _interfaceModifier.Value;

    public InterfaceModifierBuilder()
    {
        _interfaceModifier = new(this, InterfaceModifier.None);
    }

    public IInterfaceModifierBuilder UseInterfaceModifier(InterfaceModifier modifier) => _interfaceModifier.Use(modifier);

    public override IEnumerable<IDiagnostic> Validate()
    {
        yield break;
    }
}

[Flags]
public enum InterfaceModifier
{
    None = (int)Modifier.None,
    Unsafe = (int)Modifier.Unsafe
}
