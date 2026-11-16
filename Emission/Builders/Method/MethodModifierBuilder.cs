using Emission.Validation;

namespace Emission.Builders;
public class MethodModifierBuilder : ModifierBuilder<MethodModifierBuilder, IMethodModifierBuilder>, IMethodModifierBuilder
{
    private readonly SimpleBuilderProperty<MethodModifierBuilder, MethodModifier> _methodModifier;
    protected override Modifier Modifier => (Modifier)MethodModifier;
    public MethodModifier MethodModifier => _methodModifier.Value;

    public MethodModifierBuilder()
    {
        _methodModifier = new(this, MethodModifier.None);
    }

    public IMethodModifierBuilder UseMethodModifier(MethodModifier modifier) => _methodModifier.Use(modifier);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (Modifier.HasFlag(Modifier.Virtual) && Modifier.HasFlag(Modifier.Override))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Method has conflicting modifiers: {Modifier.Virtual} and {Modifier.Override}");

        if (Modifier.HasFlag(Modifier.Abstract) && Modifier.HasFlag(Modifier.Override))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Method has conflicting modifiers: {Modifier.Abstract} and {Modifier.Override}");

        if (Modifier.HasFlag(Modifier.Sealed) && !Modifier.HasFlag(Modifier.Override))
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Method has invalid modifier: {Modifier.Sealed}");
    }
}

[Flags]
public enum MethodModifier
{
    None = (int)Modifier.None,
    Abstract = (int)Modifier.Abstract,
    Async = (int)Modifier.Async,
    Extern = (int)Modifier.Extern,
    Override = (int)Modifier.Override,
    Sealed = (int)Modifier.Sealed,
    Static = (int)Modifier.Static,
    Unsafe = (int)Modifier.Unsafe,
    Virtual = (int)Modifier.Virtual
}
