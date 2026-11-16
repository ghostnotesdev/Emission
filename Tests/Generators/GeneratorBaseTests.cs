using System.Collections.Immutable;
using Emission.Context;
using Emission.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Generators;

public class GeneratorBaseTests
{
    [Fact]
    public void Initialize_ExecutesDerivedGeneratorAndAddsSource()
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new TestGenerator());
        driver = driver.RunGenerators(CSharpCompilation.Create("Generated"));

        var result = driver.GetRunResult();

        Assert.Empty(result.Diagnostics);
        Assert.Contains(result.Results.SelectMany(run => run.GeneratedSources), source => source.HintName == "generated.g.cs");
    }

    private sealed class TestGenerator : GeneratorBase
    {
        protected override Func<AdditionalText, bool> AdditionalTextsFilter => _ => true;

        protected override void Execute(IGeneratorContext context)
        {
            context.RegisterSource("generated", "public class Generated { }");
        }
    }
}
