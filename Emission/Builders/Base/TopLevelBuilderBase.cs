using Emission.Types;
using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
public abstract class TopLevelBuilderBase<TImplementation, TInterface> : BuilderBase, ITopLevelBuilder<TInterface>
    where TImplementation : TopLevelBuilderBase<TImplementation, TInterface>, TInterface
    where TInterface : ITopLevelBuilder<TInterface>
{
    private readonly BuilderNamespaceProperty<TImplementation> _namespace;
    private readonly BuilderIdentifierProperty<TImplementation> _name;

    public virtual string Namespace => _namespace.Value;
    public virtual string Name => _name.Value;
    public TypeReference Reference => new TypeReference.Generated($"global::{Namespace}.{Name}");

    public TopLevelBuilderBase()
    {
        _namespace = new((TImplementation)this);
        _name = new((TImplementation)this);
    }

    public void BuildType(ICompilationStringBuilder icsb) => icsb.Append(Reference!.Tokenize());
    public TInterface UseNamespace(string ns) => _namespace.Use(ns);
    public TInterface UseName(string name) => _name.Use(name);
    public ITypeBuilder UseType(TypeReference typeReference) => throw new InvalidOperationException(UseTypeError);
    public ITypeBuilder UseType(TypeResolutionResult resolutionResult) => throw new InvalidOperationException(UseTypeError);
    public ITypeBuilder UseType(ITypeSymbol symbol) => throw new InvalidOperationException(UseTypeError);
    public ITypeBuilder UseType(SymbolLookupResult symbolResult) => throw new InvalidOperationException(UseTypeError);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _namespace.Validate(Namespace))
            yield return diag;

        foreach (var diag in _name.Validate(Name, nameof(Name)))
            yield return diag;
    }

    protected void PreConfigureNamespace<TDestination>(ITopLevelBuilder<TDestination> builder)
        where TDestination : ITopLevelBuilder<TDestination>
    {
        if (string.IsNullOrWhiteSpace(Namespace))
            throw new InvalidOperationException(
                $"{nameof(Namespace)} must be configured before adding nested types.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException(
                $"{nameof(Name)} must be configured before adding nested types.");

        builder.UseNamespace(Namespace + "." + Name);
    }

    private string UseTypeError => $".{nameof(UseType)}() cannot be called on {typeof(TInterface).Name}. " +
        $"It is inferred from the {nameof(Namespace)} and {nameof(Name)} provided. " +
        "ITypeBuilder inheritance is provided as a convienience so top level builders can more easily be referenced as types for other builders.";
}
