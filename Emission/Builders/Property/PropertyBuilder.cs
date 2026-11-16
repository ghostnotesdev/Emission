using Emission.Validation;

namespace Emission.Builders;
public class PropertyBuilder : BuilderBase, IPropertyBuilder
{
    private readonly BuilderIdentifierProperty<PropertyBuilder> _name;
    private readonly BuilderSwitchProperty<PropertyBuilder> _interfaceProperty;
    private readonly BuilderProperty<PropertyBuilder, IPropertyModifierBuilder, PropertyModifierBuilder> _modifier;
    private readonly BuilderProperty<PropertyBuilder, ITypeBuilder, TypeBuilder> _type;
    private readonly BuilderProperty<PropertyBuilder, IGetterBuilder, GetterBuilder> _getter;
    private readonly BuilderProperty<PropertyBuilder, ISetterBuilder, SetterBuilder> _setter;
    private readonly OptionalBuilderProperty<PropertyBuilder, IAssignmentBuilder, AssignmentBuilder> _assignment;

    public virtual string PropertyName => _name.Value;
    public bool IsInterfaceProperty => _interfaceProperty.Value;
    public virtual IPropertyModifierBuilder Modifier => _modifier.Builder;
    public virtual ITypeBuilder PropertyType => _type.Builder;
    public virtual IGetterBuilder Getter => _getter.Builder;
    public virtual ISetterBuilder Setter => _setter.Builder;
    public virtual IAssignmentBuilder? Assignment => _assignment.Builder;
    public bool IsAutoAccessor => Getter.IsAuto && Setter.IsAuto;

    public override string DiagnosticName => PropertyName ?? "<unnamed property>";

    public PropertyBuilder()
    {
        _name = new(this);
        _interfaceProperty = new(this);
        _modifier = new(this);
        _type = new(this);
        _getter = new(this);
        _setter = new(this);
        _assignment = new(this);
    }

    public IPropertyBuilder ConfigureModifiers(Action<IPropertyModifierBuilder> configure) => _modifier.Configure(configure);
    public IPropertyBuilder UseModifiers(IPropertyModifierBuilder builder) => _modifier.Use(builder);
    public IPropertyBuilder UseName(string propertyName) => _name.Use(propertyName);
    public IPropertyBuilder ConfigureType(Action<ITypeBuilder> configure) => _type.Configure(configure);
    public IPropertyBuilder UseType(ITypeBuilder builder) => _type.Use(builder);
    public IPropertyBuilder ConfigureGetter(Action<IGetterBuilder> configure) => _getter.Configure(configure);
    public IPropertyBuilder UseGetter(IGetterBuilder builder) => _getter.Use(builder);
    public IPropertyBuilder ConfigureSetter(Action<ISetterBuilder> configure) => _setter.Configure(configure);
    public IPropertyBuilder UseSetter(ISetterBuilder builder) => _setter.Use(builder);
    public IPropertyBuilder ConfigureAssignment(Action<IAssignmentBuilder> configure) => _assignment.Configure(configure);
    public IPropertyBuilder UseAssignment(IAssignmentBuilder builder) => _assignment.Use(builder);
    public IPropertyBuilder AsInterfaceProperty() => _interfaceProperty.As();

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _name.Validate(PropertyName, nameof(PropertyName)))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Modifier.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(PropertyType.Validate()))
            yield return diag;

        if (string.IsNullOrWhiteSpace(PropertyName))
        {
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"{nameof(PropertyName)} cannot be null or whitespace.");
        }

        if (Setter.Modifier != SetterAccessModifier.None && (!Getter.IsAuto || !Setter.IsAuto))
            yield return new InvalidBuilderConfigurationDiagnostic(this, "Setter access modifier can only be used with auto getter/setter.");

        if (Assignment is not null && !IsAutoAccessor)
            yield return new InvalidBuilderConfigurationDiagnostic(this, "Assignment can only be used in auto getter/setter mode.");

        if (Assignment is not null)
            foreach (var diag in PropagateDiagnostics(Assignment.Validate()))
                yield return diag;

        if (IsInterfaceProperty && Modifier.AccessModifier != AccessModifier.Public)
            yield return new InvalidBuilderConfigurationDiagnostic(this, "Interface properties must be public.");
    }

    public void BuildContract(ICompilationStringBuilder icsb)
    {
        PropertyType.BuildType(icsb);
        icsb.EnsureSpace()
            .Append(PropertyName)
            .AppendLine(" { get; }");
    }

    public void BuildImplementation(ICompilationStringBuilder icsb)
    {
        Modifier.Build(icsb);
        icsb.EnsureSpace();
        PropertyType.BuildType(icsb);
        icsb.EnsureSpace()
            .Append(PropertyName);

        if (IsAutoAccessor)
        {
            icsb.Append(" { ");
            Getter.Build(icsb);
            icsb.EnsureSpace();
            Setter.Build(icsb);
            icsb.Append(" }");
            Assignment?.Build(icsb);
        }
        else if (Getter.Expression is not null && Setter.IsAuto)
        {
            Getter.Build(icsb, false);
            icsb.EnsureNewLine();
        }
        else
        {
            icsb.EnsureNewLine()
                .AppendLine("{")
                .IncreaseIndent();

            Getter.Build(icsb);
            if (!Setter.IsAuto)
                Setter.Build(icsb.EnsureNewLine());

            icsb.DecreaseIndent()
                .AppendLine("}");
        }
    }
}
