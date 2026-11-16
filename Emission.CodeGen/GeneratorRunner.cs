using Emission.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Emission.CodeGen;

public class GeneratorRunner
{
    private readonly Lazy<List<IIncrementalGenerator>> _incrementalGenerators = new(GetGenerators);

    public IReadOnlyList<IIncrementalGenerator> IncrementalGenerators => _incrementalGenerators.Value;

    public IEnumerable<BuiltSource> Run()
    {
        CSharpCompilation compilation = CreateCompilation();
        IEnumerable<ISourceGenerator> generators = IncrementalGenerators.Select(x => x.AsSourceGenerator());
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators);
        driver = driver.RunGenerators(compilation);
        GeneratorDriverRunResult result = driver.GetRunResult();

        if (result.Diagnostics.Length > 0)
        {
            Console.WriteLine("Emission.CodeGen has diagnostic errors:");
            foreach (var diag in result.Diagnostics)
            {
                Console.WriteLine(diag.ToString());
            }
        }

        foreach (GeneratorRunResult generatorResult in result.Results)
        {
            foreach (GeneratedSourceResult source in generatorResult.GeneratedSources)
            {
                yield return new BuiltSource(source.HintName, source.SourceText.ToString());
            }
        }
    }

    private static List<IIncrementalGenerator> GetGenerators() => [
        .. Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                typeof(IIncrementalGenerator).IsAssignableFrom(t)
                )
            .Select(t => (IIncrementalGenerator)Activator.CreateInstance(t)!)
        ];

    private static CSharpCompilation CreateCompilation()
    {
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location));

        return CSharpCompilation.Create(
            assemblyName: "Emission.CodeGen",
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
