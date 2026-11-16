using Microsoft.CodeAnalysis;
using Emission.Context;
using Emission.Validation;
using Emission.Types;

namespace Emission.Generators;

public abstract class GeneratorBase : GeneratorBase<GeneratorContext, IGeneratorContext, ITypeResolver>;

public abstract class GeneratorBase<TGeneratorContextImpl, TGeneratorContextIntf, TTypeResolverIntf> : IIncrementalGenerator
    where TGeneratorContextImpl : TGeneratorContextIntf, new()
    where TGeneratorContextIntf : IGeneratorContext<TTypeResolverIntf>
    where TTypeResolverIntf : ITypeResolver
{
    private readonly CancellationTokenSource _tokenSource = new();

    // If it takes longer than 5 minutes, assume something went wrong and cancel.
    // Can be overriden by derived classes to specify a different timeout.
    protected virtual TimeSpan GeneratorTimeout => TimeSpan.FromMinutes(5);
    protected virtual Func<AdditionalText, bool> AdditionalTextsFilter { get; } = (_) => false;

    protected abstract void Execute(TGeneratorContextIntf context);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _tokenSource.CancelAfter(GeneratorTimeout);

        IncrementalValueProvider<Compilation> compilation = context.CompilationProvider;
        var files = context.AdditionalTextsProvider
            .Where(AdditionalTextsFilter)
            .Collect();

        var input = files.Combine(compilation);

        context.RegisterSourceOutput(input, (spc, source) =>
        {
            try
            {
                TGeneratorContextImpl context = new();
                context.Initialize(spc, source.Right, source.Left, _tokenSource.Token);
                Execute(context);
            }
            catch (Exception ex)
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.UnhandledError, Location.None, GetType().Name, ex.ToString());
                spc.ReportDiagnostic(diagnostic);
            }
        });
    }
}
