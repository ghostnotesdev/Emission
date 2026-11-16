using Emission.Builders;
using Emission.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Emission.Context;

public interface IGeneratorContext<TTypeResolver> where TTypeResolver : ITypeResolver
{
    TTypeResolver TypeResolver { get; }
    SourceProductionContext Context { get; }
    Compilation Compilation { get; }
    CancellationToken CancellationToken { get; }
    ImmutableArray<AdditionalText> AdditionalTexts { get; }

    void Initialize(SourceProductionContext context, Compilation compilation, ImmutableArray<AdditionalText> additionalTexts, CancellationToken cancellationToken);
    ISourceBuilder CreateBuilder();
    IGeneratorContext<TTypeResolver> AddSource(ISourceBuilder builder);
    IGeneratorContext<TTypeResolver> AddClassSource(IClassBuilder builder);
    IGeneratorContext<TTypeResolver> AddEnumSource(IEnumBuilder builder);
    IGeneratorContext<TTypeResolver> AddInterfaceSource(IInterfaceBuilder builder);
    void BuildSource();
    void RegisterSource(string hintName, string source);
    void Report(Diagnostic diagnostic);
}

public interface IGeneratorContext : IGeneratorContext<ITypeResolver>;
