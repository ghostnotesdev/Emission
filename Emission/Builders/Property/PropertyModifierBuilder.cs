using Emission.Validation;

namespace Emission.Builders;
public class PropertyModifierBuilder : ModifierBuilder<PropertyModifierBuilder, IPropertyModifierBuilder>, IPropertyModifierBuilder
{
    private readonly SimpleBuilderProperty<PropertyModifierBuilder, PropertyModifier> _propertyModifier;
    protected override Modifier Modifier => (Modifier)PropertyModifier;
    public virtual PropertyModifier PropertyModifier => _propertyModifier.Value;

    public PropertyModifierBuilder()
    {
        _propertyModifier = new(this, PropertyModifier.None);
    }

    public IPropertyModifierBuilder UsePropertyModifier(PropertyModifier modifier) => _propertyModifier.Use(modifier);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (PropertyModifier.HasFlag(PropertyModifier.Virtual) && PropertyModifier.HasFlag(PropertyModifier.Override))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Property has conflicting modifiers: {PropertyModifier.Virtual} and {PropertyModifier.Override}");

        if (PropertyModifier.HasFlag(PropertyModifier.Abstract) && PropertyModifier.HasFlag(PropertyModifier.Override))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Property has conflicting modifiers: {PropertyModifier.Abstract} and {PropertyModifier.Override}");

        if (PropertyModifier.HasFlag(PropertyModifier.Sealed) && !PropertyModifier.HasFlag(PropertyModifier.Override))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Property has invalid modifier: {PropertyModifier.Sealed}");
    }
}

[Flags]
public enum PropertyModifier
{
    None = (int)Modifier.None,
    Abstract = (int)Modifier.Abstract,
    Extern = (int)Modifier.Extern,
    Override = (int)Modifier.Override,
    Sealed = (int)Modifier.Sealed,
    Static = (int)Modifier.Static,
    Unsafe = (int)Modifier.Unsafe,
    Virtual = (int)Modifier.Virtual
}
