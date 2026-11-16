using Emission.Builders;
using Emission.Extensions;
using Emission.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Emission.Context;

public class GeneratorContext : GeneratorContext<TypeResolver, ITypeResolver>, IGeneratorContext;

// pls Microsoft
// where TTypeResolverImpl : TTypeResolverIntf, new(Microsoft.CodeAnalysis.Compilation)
public class GeneratorContext<TTypeResolverImpl, TTypeResolverIntf> : IGeneratorContext<TTypeResolverIntf>
    where TTypeResolverImpl : TTypeResolverIntf, new()
    where TTypeResolverIntf : ITypeResolver
{
    protected const string EXTENSION = ".g.cs";
    private const string _invalidError = nameof(Initialize) + " must be called before using " + nameof(GeneratorContext<,>);
    private TTypeResolverImpl? _resolver;
    private SourceProductionContext? _spc;
    private Compilation? _compilation;
    private ImmutableArray<AdditionalText>? _additionalTexts;
    private CancellationToken? _token;
    protected List<Diagnostic> _errors = [];
    protected List<ISourceBuilder> _source = [];

    public TTypeResolverIntf TypeResolver => _resolver ?? throw new InvalidOperationException(_invalidError);
    public SourceProductionContext Context => _spc ?? throw new InvalidOperationException(_invalidError);
    public Compilation Compilation => _compilation ?? throw new InvalidOperationException(_invalidError);
    public ImmutableArray<AdditionalText> AdditionalTexts => _additionalTexts ?? throw new InvalidOperationException(_invalidError);
    public CancellationToken CancellationToken => _token ?? throw new InvalidOperationException(_invalidError);

    public void Initialize(SourceProductionContext context, Compilation compilation, ImmutableArray<AdditionalText> additionalTexts, CancellationToken cancellationToken)
    {
        _resolver = new TTypeResolverImpl();
        _resolver.Initialize(compilation);
        _spc = context;
        _compilation = compilation;
        _additionalTexts = additionalTexts;
        _token = cancellationToken;
    }

    public virtual ISourceBuilder CreateBuilder() => new SourceBuilder();

    public virtual IGeneratorContext<TTypeResolverIntf> AddSource(ISourceBuilder builder)
    {
        _source.Add(builder);
        return this;
    }

    public virtual IGeneratorContext<TTypeResolverIntf> AddClassSource(IClassBuilder builder)
    {
        var src = CreateBuilder();
        src.WithClass(builder);
        return AddSource(src);
    }

    public virtual IGeneratorContext<TTypeResolverIntf> AddEnumSource(IEnumBuilder builder)
    {
        var src = CreateBuilder();
        src.WithEnum(builder);
        return AddSource(src);
    }

    public virtual IGeneratorContext<TTypeResolverIntf> AddInterfaceSource(IInterfaceBuilder builder)
    {
        var src = CreateBuilder();
        src.WithInterface(builder);
        return AddSource(src);
    }

    public void BuildSource()
    {
        if (Validate())
        {
            foreach (var builder in _source)
            {
                RegisterSource(builder.Build());
            }
        }
    }

    private void RegisterSource(BuiltSource build) => RegisterSource(build.HintName, build.Source);

    public virtual void RegisterSource(string hintName, string source)
    {
        if (!hintName.EndsWith(EXTENSION))
            hintName += EXTENSION;
        Context.AddSource(hintName, source);
    }

    public virtual void Report(Diagnostic diagnostic)
    {
        if (diagnostic.Severity == DiagnosticSeverity.Error)
            _errors.Add(diagnostic);

        Context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Validates all the source files registered at once.
    /// </summary>
    /// <returns>True for valid. False if there were errors.</returns>
    private bool Validate()
    {
        _errors.Clear();
        foreach (var builder in _source)
        {
            foreach (var diag in builder.Validate())
            {
                Report(diag.ToDiagnostic());
            }
        }

        return _errors.Count == 0;
    }
}
