using Emission.Validation;

namespace Emission.Builders;
public class ClassModifierBuilder : ModifierBuilder<ClassModifierBuilder, IClassModifierBuilder>, IClassModifierBuilder
{
    private readonly SimpleBuilderProperty<ClassModifierBuilder, ClassModifier> _classModifier;

    public ClassModifier ClassModifier => _classModifier.Value;
    protected override Modifier Modifier => (Modifier)ClassModifier;

    public ClassModifierBuilder()
    {
        _classModifier = new(this, ClassModifier.None);
    }

    public IClassModifierBuilder UseClassModifier(ClassModifier modifier) => _classModifier.Use(modifier);

    public override IEnumerable<IDiagnostic> Validate()
    {
        int declarationModifiers = 0;

        if (ClassModifier.HasFlag(ClassModifier.Abstract))
            declarationModifiers++;

        if (ClassModifier.HasFlag(ClassModifier.Sealed))
            declarationModifiers++;

        if (ClassModifier.HasFlag(ClassModifier.Static))
            declarationModifiers++;

        if (declarationModifiers > 1)
        {
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Class has conflicting modifiers: {ClassModifier}");
        }
    }
}

[Flags]
public enum ClassModifier
{
    None = (int)Modifier.None,
    Abstract = (int)Modifier.Abstract,
    Sealed = (int)Modifier.Sealed,
    Static = (int)Modifier.Static,
    Unsafe = (int)Modifier.Unsafe
}
