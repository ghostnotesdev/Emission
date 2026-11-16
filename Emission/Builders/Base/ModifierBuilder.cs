namespace Emission.Builders;
public abstract class ModifierBuilder<TImplementation, TInterface> : BuilderBase, IModifierBuilder<TInterface>
    where TImplementation : ModifierBuilder<TImplementation, TInterface>, TInterface
    where TInterface : IModifierBuilder<TInterface>
{
    private readonly SimpleBuilderProperty<TImplementation, AccessModifier> _accessModifier;
    private readonly SimpleBuilderProperty<TImplementation, Modifier> _modifier;
    public virtual AccessModifier AccessModifier => _accessModifier.Value;
    protected virtual Modifier Modifier => _modifier.Value;

    public ModifierBuilder()
    {
        _accessModifier = new((TImplementation)this, AccessModifier.None);
        _modifier = new((TImplementation)this, Modifier.None);
    }

    public TInterface UseAccessModifier(AccessModifier modifier) => _accessModifier.Use(modifier);
    protected TInterface UseModifier(Modifier modifier) => _modifier.Use(modifier);

    public void Build(ICompilationStringBuilder icsb)
    {
        if (AccessModifier != AccessModifier.None)
            icsb.EnsureSpace()
                .Append(GetAccessModifierSource(AccessModifier));

        if (Modifier != Modifier.None)
            AppendModifierSource(icsb, Modifier);
    }

    private string GetAccessModifierSource(AccessModifier modifier) => modifier switch
    {
        AccessModifier.Internal => "internal",
        AccessModifier.Private => "private",
        AccessModifier.Protected => "protected",
        AccessModifier.Public => "public",
        AccessModifier.ProtectedInternal => "protected internal",
        AccessModifier.PrivateProtected => "private protected",
        AccessModifier.File => "file",
        _ => string.Empty
    };

    private void AppendModifierSource(ICompilationStringBuilder icsb, Modifier modifier)
    {
        foreach (var mod in ModifierOrder)
        {
            if (modifier.HasFlag(mod.Modifier))
            {
                icsb.EnsureSpace().Append(mod.Source);
            }
        }
    }

    private static readonly (Modifier Modifier, string Source)[] ModifierOrder =
    [
        (Modifier.Static, "static"),
        (Modifier.Abstract, "abstract"),
        (Modifier.Closed, "closed"),
        (Modifier.Sealed, "sealed"),
        (Modifier.Const, "const"),
        (Modifier.ReadOnly, "readonly"),
        (Modifier.Volatile, "volatile"),
        (Modifier.Virtual, "virtual"),
        (Modifier.Override, "override"),
        (Modifier.Extern, "extern"),
        (Modifier.Safe, "safe"),
        (Modifier.Unsafe, "unsafe"),
        (Modifier.Async, "async"),
    ];
}

public enum AccessModifier
{
    None = 0,
    Internal = 1,
    Private = 2,
    Protected = 3,
    Public = 4,
    ProtectedInternal = 5,
    PrivateProtected = 6,
    File = 7,
}

[Flags]
public enum Modifier
{
    None = 0,
    Abstract = 1 << 0,
    Async = 1 << 1,
    Closed = 1 << 2,
    Const = 1 << 3,
    Extern = 1 << 4,
    Override = 1 << 5,
    ReadOnly = 1 << 6,
    Safe = 1 << 7,
    Sealed = 1 << 8,
    Static = 1 << 9,
    Unsafe = 1 << 10,
    Virtual = 1 << 11,
    Volatile = 1 << 12
}
