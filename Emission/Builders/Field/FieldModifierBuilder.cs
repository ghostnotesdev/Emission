using Emission.Validation;

namespace Emission.Builders;
public class FieldModifierBuilder : ModifierBuilder<FieldModifierBuilder, IFieldModifierBuilder>, IFieldModifierBuilder
{
    private readonly SimpleBuilderProperty<FieldModifierBuilder, FieldModifier> _fieldModifier;
    protected override Modifier Modifier => (Modifier)FieldModifier;
    public FieldModifier FieldModifier => _fieldModifier.Value;

    public FieldModifierBuilder()
    {
        _fieldModifier = new(this, FieldModifier.None);
    }

    public IFieldModifierBuilder UseFieldModifier(FieldModifier modifier) => _fieldModifier.Use(modifier);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (FieldModifier.HasFlag(FieldModifier.Const) && FieldModifier != FieldModifier.Const)
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Field modifier {Modifier.Const} cannot be combined with other modifiers.");

        if (FieldModifier.HasFlag(FieldModifier.ReadOnly) && FieldModifier.HasFlag(FieldModifier.Volatile))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Field has conflicting modifiers: {Modifier.ReadOnly} and {Modifier.Volatile}");
    }
}

[Flags]
public enum FieldModifier
{
    None = (int)Modifier.None,
    Const = (int)Modifier.Const,
    ReadOnly = (int)Modifier.ReadOnly,
    Static = (int)Modifier.Static,
    Unsafe = (int)Modifier.Unsafe,
    Volatile = (int)Modifier.Volatile
}
