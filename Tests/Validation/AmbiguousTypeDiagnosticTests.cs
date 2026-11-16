using Emission.Validation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Validation;

public class AmbiguousTypeDiagnosticTests
{
    [Fact]
    public void Constructor_ReportsCandidates()
    {
        var symbol = CSharpCompilation.Create(
            "Diagnostics",
            [CSharpSyntaxTree.ParseText("public class Person { }")],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)])
            .GetTypeByMetadataName("Person")!;
        var diagnostic = new AmbiguousTypeDiagnostic("Person", 0, [symbol]);

        Assert.Equal("Person", diagnostic.Query);
        Assert.Equal(0, diagnostic.Arity);
        Assert.Contains("Person", diagnostic.Message);
    }
}
