using Emission.Validation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Validation;

public class MissingMethodDiagnosticTests
{
    [Fact]
    public void Constructor_ReportsSymbolAndMappings()
    {
        var symbol = CSharpCompilation.Create(
            "Diagnostics",
            [CSharpSyntaxTree.ParseText("public class Person { }")],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)])
            .GetTypeByMetadataName("Person")!;
        var diagnostic = new MissingMethodDiagnostic(symbol, []);

        Assert.Same(symbol, diagnostic.Symbol);
        Assert.Contains("Person", diagnostic.Message);
        Assert.Empty(diagnostic.Mappings);
    }
}
