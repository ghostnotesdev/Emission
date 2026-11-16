using Emission.Validation;

namespace Emission.Builders;
public class ClassBuilder : TopLevelBuilderBase<ClassBuilder, IClassBuilder>, IClassBuilder
{
    private readonly BuilderProperty<ClassBuilder, IClassModifierBuilder, ClassModifierBuilder> _modifier;
    private readonly BuilderProperty<ClassBuilder, IClassInheritanceBuilder, ClassInheritanceBuilder> _inheritance;
    private readonly BuilderProperty<ClassBuilder, IGenericArgumentsBuilder, GenericArgumentsBuilder> _generics;
    private readonly BuilderEnumerableProperty<ClassBuilder, IEnumBuilder, EnumBuilder> _enums;
    private readonly BuilderEnumerableProperty<ClassBuilder, IFieldBuilder, FieldBuilder> _fields;
    private readonly BuilderEnumerableProperty<ClassBuilder, IPropertyBuilder, PropertyBuilder> _properties;
    private readonly BuilderEnumerableProperty<ClassBuilder, IConstructorBuilder, ConstructorBuilder> _constructors;
    private readonly BuilderEnumerableProperty<ClassBuilder, IMethodBuilder, MethodBuilder> _methods;
    private readonly BuilderEnumerableProperty<ClassBuilder, IClassBuilder, ClassBuilder> _classes;

    public virtual IClassModifierBuilder ModifierBuilder => _modifier.Builder;
    public virtual IClassInheritanceBuilder InheritanceBuilder => _inheritance.Builder;
    public virtual IGenericArgumentsBuilder GenericArgumentsBuilder => _generics.Builder;
    public virtual IEnumerable<IEnumBuilder> Enums => _enums.Entries;
    public virtual IEnumerable<IFieldBuilder> Fields => _fields.Entries;
    public virtual IEnumerable<IPropertyBuilder> Properties => _properties.Entries;
    public virtual IEnumerable<IConstructorBuilder> Constructors => _constructors.Entries;
    public virtual IEnumerable<IMethodBuilder> Methods => _methods.Entries;
    public virtual IEnumerable<IClassBuilder> Classes => _classes.Entries;
    public override string DiagnosticName => !string.IsNullOrWhiteSpace(Name) ? Name : "<unnamed class>";

    public ClassBuilder()
    {
        _modifier = new(this);
        _inheritance = new(this);
        _generics = new(this);
        _enums = new(this, PreConfigureNamespace);
        _fields = new(this);
        _properties = new(this);
        _constructors = new(this);
        _methods = new(this);
        _classes = new(this, PreConfigureNamespace);
    }

    public IClassBuilder ConfigureModifiers(Action<IClassModifierBuilder> configure) => _modifier.Configure(configure);
    public IClassBuilder UseModifiers(IClassModifierBuilder builder) => _modifier.Use(builder);
    public IClassBuilder UseEnums(IEnumerable<IEnumBuilder> builders) => _enums.Use(builders);
    public IClassBuilder WithEnum(Action<IEnumBuilder> configure) => _enums.With(configure);
    public IClassBuilder WithEnum(IEnumBuilder builder) => _enums.With(builder);
    public IClassBuilder UseFields(IEnumerable<IFieldBuilder> builders) => _fields.Use(builders);
    public IClassBuilder WithField(Action<IFieldBuilder> configure) => _fields.With(configure);
    public IClassBuilder WithField(IFieldBuilder builder) => _fields.With(builder);
    public IClassBuilder UseProperties(IEnumerable<IPropertyBuilder> builders) => _properties.Use(builders);
    public IClassBuilder WithProperty(Action<IPropertyBuilder> configure) => _properties.With(configure);
    public IClassBuilder WithProperty(IPropertyBuilder builder) => _properties.With(builder);
    public IClassBuilder UseConstructors(IEnumerable<IConstructorBuilder> builders) => _constructors.Use(builders);
    public IClassBuilder WithConstructor(Action<IConstructorBuilder> configure) => _constructors.With(configure);
    public IClassBuilder WithConstructor(IConstructorBuilder builder) => _constructors.With(builder);
    public IClassBuilder UseMethods(IEnumerable<IMethodBuilder> builders) => _methods.Use(builders);
    public IClassBuilder WithMethod(Action<IMethodBuilder> configure) => _methods.With(configure);
    public IClassBuilder WithMethod(IMethodBuilder builder) => _methods.With(builder);
    public IClassBuilder UseClasses(IEnumerable<IClassBuilder> builders) => _classes.Use(builders);
    public IClassBuilder WithClass(Action<IClassBuilder> configure) => _classes.With(configure);
    public IClassBuilder WithClass(IClassBuilder builder) => _classes.With(builder);
    public IClassBuilder ConfigureInheritance(Action<IClassInheritanceBuilder> configure) => _inheritance.Configure(configure);
    public IClassBuilder UseInheritance(IClassInheritanceBuilder builder) => _inheritance.Use(builder);
    public IClassBuilder ConfigureGenericArguments(Action<IGenericArgumentsBuilder> configure) => _generics.Configure(configure);
    public IClassBuilder UseGenericArguments(IGenericArgumentsBuilder builder) => _generics.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in base.Validate())
            yield return diag;

        foreach (var diag in PropagateDiagnostics(ModifierBuilder.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(InheritanceBuilder.Validate()))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Enums.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Fields.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Properties.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Constructors.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Methods.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Classes.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(GenericArgumentsBuilder.Validate()))
            yield return diag;

        if (Methods.Any(m => m.IsExtensionMethod))
        {
            if (ModifierBuilder.AccessModifier != AccessModifier.Public)
                yield return new InvalidBuilderConfigurationDiagnostic(this, "Classes containing extension methods must be public");
            if (ModifierBuilder.ClassModifier != ClassModifier.Static)
                yield return new InvalidBuilderConfigurationDiagnostic(this, "Classes containing extension methods must be static");
        }
    }

    public void BuildClass(ICompilationStringBuilder icsb)
    {
        ModifierBuilder.Build(icsb);
        icsb.EnsureSpace()
            .Append("class " + Name);
        GenericArgumentsBuilder.BuildDeclaration(icsb);
        InheritanceBuilder.Build(icsb);

        if (GenericArgumentsBuilder.Arguments.Any())
        {
            icsb.AppendLine();
            icsb.IncreaseIndent();
            GenericArgumentsBuilder.BuildConstraints(icsb);
            icsb.AppendLine();
            icsb.DecreaseIndent();
        }

        icsb.EnsureNewLine()
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

        foreach (var enumBuilder in Enums)
        {
            newLineIfNeccessary();
            enumBuilder.BuildEnum(icsb);
        }

        foreach (var fieldBuilder in Fields)
        {
            newLineIfNeccessary();
            fieldBuilder.Build(icsb);
        }

        foreach (var constructorBuilder in Constructors)
        {
            newLineIfNeccessary();
            constructorBuilder.Build(icsb, Name);
        }

        foreach (var propertyBuilder in Properties)
        {
            newLineIfNeccessary();
            propertyBuilder.BuildImplementation(icsb);
        }

        foreach (var methodBuilder in Methods)
        {
            newLineIfNeccessary();
            methodBuilder.BuildImplementation(icsb);
        }

        foreach (var classBuilder in Classes)
        {
            newLineIfNeccessary();
            classBuilder.BuildClass(icsb);
        }

        icsb.DecreaseIndent()
            .AppendLine("}");
    }
}
