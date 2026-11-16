using Emission.Builders;
using Emission.Validation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Validation;

public class MissingConstructorDiagnosticTests
{
    [Fact]
    public void Constructor_ReportsSymbolAndMappings()
    {
        var symbol = GetPersonType();
        var diagnostic = new MissingConstructorDiagnostic(
            symbol,
            [new StatementParameterBuilder()
                .ConfigureType(type => type.UseType(new Emission.Types.TypeReference.Generated("string")))
                .UseStatement(new StatementBuilder().UseStatement("value"))]);

        Assert.Same(symbol, diagnostic.Symbol);
        Assert.Contains("Person", diagnostic.Message);
        Assert.Single(diagnostic.Mappings);
    }

    private static INamedTypeSymbol GetPersonType() => CSharpCompilation.Create(
        "Diagnostics",
        [CSharpSyntaxTree.ParseText("public class Person { }")],
        [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)])
        .GetTypeByMetadataName("Person")!;
}
