using Emission.Extensions;
using Emission.Validation;

namespace Emission.Builders;
public class SetterBuilder : PropertyAccessorBuilder<SetterBuilder, ISetterBuilder>, ISetterBuilder
{
    private readonly SimpleBuilderProperty<SetterBuilder, SetterAccessModifier> _setterModifier;
    protected override string AccessorString => Modifier != SetterAccessModifier.None ? Modifier.ToString().ToCamelCase() + " set" : "set";
    public virtual SetterAccessModifier Modifier => _setterModifier.Value;

    public SetterBuilder()
    {
        _setterModifier = new(this, SetterAccessModifier.None);
    }

    public ISetterBuilder UseAccessModifier(SetterAccessModifier modifier) => _setterModifier.Use(modifier);

    public override IEnumerable<IDiagnostic> Validate()
    {
        yield break;
    }
}

public enum SetterAccessModifier
{
    None = 0,
    Internal = 1,
    Private = 2,
    Protected = 3,
    ProtectedInternal = 5,
    PrivateProtected = 6
}
