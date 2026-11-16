using Emission.Validation;

namespace Emission.Builders;
public class SourceBuilder : BuilderBase, ISourceBuilder
{
    private readonly BuilderNamespaceProperty<SourceBuilder> _namespace;
    private readonly BuilderIdentifierProperty<SourceBuilder> _hintName;
    private readonly BuilderEnumerableProperty<SourceBuilder, IEnumBuilder, EnumBuilder> _enumBuilders;
    private readonly BuilderEnumerableProperty<SourceBuilder, IInterfaceBuilder, InterfaceBuilder> _interfaceBuilders;
    private readonly BuilderEnumerableProperty<SourceBuilder, IClassBuilder, ClassBuilder> _classBuilders;
    protected readonly ICompilationStringBuilder _builder;
    private string? _builtSource;

    public virtual string Namespace => _namespace.Value;
    public virtual string HintName => _hintName.Value;
    public virtual IEnumerable<IEnumBuilder> Enums => _enumBuilders.Entries;
    public virtual IEnumerable<IInterfaceBuilder> Interfaces => _interfaceBuilders.Entries;
    public virtual IEnumerable<IClassBuilder> Classes => _classBuilders.Entries;
    public override string DiagnosticName => _hintName.Value;

    public SourceBuilder(ICompilationStringBuilder builder)
    {
        _namespace = new(this);
        _hintName = new(this);
        _enumBuilders = new(this, PreConfigureNamespace);
        _interfaceBuilders = new(this, PreConfigureNamespace);
        _classBuilders = new(this, PreConfigureNamespace);
        _builder = builder;
    }

    public SourceBuilder() : this(new CompilationStringBuilder()) { }

    public ISourceBuilder UseNamespace(string ns) => _namespace.Use(ns);
    public ISourceBuilder UseHintName(string hintName) => _hintName.Use(hintName);
    public ISourceBuilder UseEnums(IEnumerable<IEnumBuilder> builders) => _enumBuilders.Use(builders);
    public ISourceBuilder WithEnum(Action<IEnumBuilder> configure) => _enumBuilders.With(configure);
    public ISourceBuilder WithEnum(IEnumBuilder builder) => _enumBuilders.With(builder);
    public ISourceBuilder UseInterfaces(IEnumerable<IInterfaceBuilder> builders) => _interfaceBuilders.Use(builders);
    public ISourceBuilder WithInterface(Action<IInterfaceBuilder> configure) => _interfaceBuilders.With(configure);
    public ISourceBuilder WithInterface(IInterfaceBuilder builder) => _interfaceBuilders.With(builder);
    public ISourceBuilder UseClasses(IEnumerable<IClassBuilder> builders) => _classBuilders.Use(builders);
    public ISourceBuilder WithClass(Action<IClassBuilder> configure) => _classBuilders.With(configure);
    public ISourceBuilder WithClass(IClassBuilder builder) => _classBuilders.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _namespace.Validate(Namespace))
            yield return diag;

        foreach (var diag in _hintName.Validate(HintName, nameof(HintName)))
            yield return diag;

        if (!Enums.Any() && !Interfaces.Any() && !Classes.Any())
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"{nameof(SourceBuilder)} is empty and will not compile.");

        foreach (var diag in PropagateDiagnostics(Enums.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Interfaces.SelectMany(e => e.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Classes.SelectMany(e => e.Validate())))
            yield return diag;
    }

    public BuiltSource Build()
    {
        if (_builtSource is not null)
            return new(HintName, _builtSource);

        if (Validate().Any())
            throw new InvalidOperationException($"{nameof(SourceBuilder)} is in an invalid state. Call .{nameof(Validate)}() before .{nameof(Build)}() for more information.");

        _builder.AppendGeneratorHeader(Namespace);

        foreach (var enumBuilder in Enums)
        {
            enumBuilder.BuildEnum(_builder);
        }

        foreach (var interfaceBuilder in Interfaces)
        {
            interfaceBuilder.BuildInterface(_builder);
        }

        foreach (var classBuilder in Classes)
        {
            classBuilder.BuildClass(_builder);
        }

        _builtSource = _builder.ToString();
        return new(HintName, _builtSource);
    }

    private void PreConfigureNamespace<TDestination>(ITopLevelBuilder<TDestination> builder)
        where TDestination : ITopLevelBuilder<TDestination>
    {
        if (string.IsNullOrWhiteSpace(Namespace) && string.IsNullOrWhiteSpace(builder.Namespace))
            throw new InvalidOperationException(
                    $"{nameof(Namespace)} must be configured on the source builder or top level builder before being added.");

        // If the source builder doesn't have its namespace and hint name configured, pull it from a top level builder that should
        if (string.IsNullOrWhiteSpace(Namespace))
            UseNamespace(builder.Namespace);

        if (string.IsNullOrWhiteSpace(HintName))
            UseHintName(builder.Name);
        
        _builtSource = null;
    }
}
