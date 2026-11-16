using Emission.Validation;

namespace Emission.Builders;
public class InterfaceBuilder : TopLevelBuilderBase<InterfaceBuilder, IInterfaceBuilder>, IInterfaceBuilder
{
    private readonly BuilderProperty<InterfaceBuilder, IInterfaceModifierBuilder, InterfaceModifierBuilder> _modifier;
    private readonly BuilderProperty<InterfaceBuilder, IInterfaceInheritanceBuilder, InterfaceInheritanceBuilder> _inheritance;
    private readonly BuilderEnumerableProperty<InterfaceBuilder, IPropertyBuilder, PropertyBuilder> _properties;
    private readonly BuilderEnumerableProperty<InterfaceBuilder, IMethodBuilder, MethodBuilder> _methods;

    public virtual IInterfaceModifierBuilder ModifierBuilder => _modifier.Builder;
    public virtual IInterfaceInheritanceBuilder InheritanceBuilder => _inheritance.Builder;
    public virtual IEnumerable<IPropertyBuilder> Properties => _properties.Entries;
    public virtual IEnumerable<IMethodBuilder> Methods => _methods.Entries;

    public override string DiagnosticName => !string.IsNullOrWhiteSpace(Name) ? Name : "<unnamed interface>";

    public InterfaceBuilder()
    {
        _modifier = new(this);
        _inheritance = new(this);
        _properties = new(this);
        _methods = new(this);
    }

    public IInterfaceBuilder ConfigureModifiers(Action<IInterfaceModifierBuilder> configure) => _modifier.Configure(configure);
    public IInterfaceBuilder UseModifiers(IInterfaceModifierBuilder builder) => _modifier.Use(builder);
    public IInterfaceBuilder ConfigureInheritance(Action<IInterfaceInheritanceBuilder> configure) => _inheritance.Configure(configure);
    public IInterfaceBuilder UseInheritance(IInterfaceInheritanceBuilder builder) => _inheritance.Use(builder);
    public IInterfaceBuilder UseProperties(IEnumerable<IPropertyBuilder> builders) => _properties.Use(builders);
    public IInterfaceBuilder WithProperty(Action<IPropertyBuilder> configure) => _properties.With(configure);
    public IInterfaceBuilder WithProperty(IPropertyBuilder builder) => _properties.With(builder);
    public IInterfaceBuilder UseMethods(IEnumerable<IMethodBuilder> builders) => _methods.Use(builders);
    public IInterfaceBuilder WithMethod(Action<IMethodBuilder> configure) => _methods.With(configure);
    public IInterfaceBuilder WithMethod(IMethodBuilder builder) => _methods.With(builder);
    
    public static IInterfaceBuilder FromClassBuilder(IClassBuilder classBuilder)
    {
        IInterfaceBuilder interfaceBuilder = new InterfaceBuilder()
            .UseNamespace(classBuilder.Namespace)
            .UseName("I" + classBuilder.Name)
            .ConfigureModifiers(mod => mod.UseAccessModifier(classBuilder.ModifierBuilder.AccessModifier))
            .UseProperties(classBuilder.Properties.Where(p => p.IsInterfaceProperty && p.Modifier.AccessModifier == AccessModifier.Public))
            .UseMethods(classBuilder.Methods.Where(m => m.IsInterfaceMethod && m.Modifier.AccessModifier == AccessModifier.Public));

        classBuilder.ConfigureInheritance(i => i.WithImplementation(interfaceBuilder));
        return interfaceBuilder;
    }

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in base.Validate())
            yield return diag;

        foreach (var diag in PropagateDiagnostics(ModifierBuilder.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(InheritanceBuilder.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Properties.SelectMany(p => p.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Methods.SelectMany(m => m.Validate())))
            yield return diag;
    }

    public void BuildInterface(ICompilationStringBuilder icsb)
    {
        ModifierBuilder.Build(icsb);
        icsb.EnsureSpace()
            .Append("interface " + Name);
        InheritanceBuilder.Build(icsb);
        icsb.AppendLine()
            .AppendLine("{")
            .IncreaseIndent();

        // First member doesn't get a new line
        bool isFirstMember = true;
        void newLineIfNeccessary()
        {
            if (!isFirstMember)
                icsb.AppendLine();
            else
                isFirstMember = false;
        }

        foreach (var propertyBuilder in Properties)
        {
            newLineIfNeccessary();
            propertyBuilder.BuildContract(icsb);
            icsb.EnsureNewLine();
        }

        foreach (var methodBuilder in Methods)
        {
            newLineIfNeccessary();
            methodBuilder.BuildContract(icsb);
            icsb.EnsureNewLine();
        }

        icsb.DecreaseIndent()
            .AppendLine("}");
    }
}
