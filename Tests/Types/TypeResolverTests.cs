using Emission.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Types;

public class TypeResolverTests
{
    [Fact]
    public void Resolve_ByMetadataNameReturnsTypeReference()
    {
        var resolver = new TypeResolver();
        resolver.Initialize(CreateCompilation());

        var result = resolver.Resolve(TypeQuery.FromMetadataName("System.String"));

        Assert.NotNull(result.Result);
        Assert.Empty(result.Diagnostics);
        Assert.Null(result.Exception);
        Assert.Equal("string", result.Result!.Tokenize());
    }

    [Fact]
    public void Resolve_MissingNameReturnsDiagnostic()
    {
        var resolver = new TypeResolver();
        resolver.Initialize(CreateCompilation());

        var result = resolver.Resolve(TypeQuery.FromName("DoesNotExist"));

        Assert.Null(result.Result);
        Assert.IsType<Emission.Validation.MissingTypeDiagnostic>(Assert.Single(result.Diagnostics));
        var exception = Assert.IsType<Emission.Validation.DiagnosticAggregateException>(result.Exception);
        Assert.Contains("DoesNotExist", exception.Message);
        Assert.IsType<Emission.Validation.DiagnosticException>(Assert.Single(exception.InnerExceptions));
    }

    [Fact]
    public void Resolve_CachesEquivalentQueries()
    {
        var resolver = new TypeResolver();
        resolver.Initialize(CreateCompilation());
        var query = TypeQuery.FromMetadataName("System.String");

        var first = resolver.Resolve(query);
        var second = resolver.Resolve(TypeQuery.FromMetadataName("System.String"));

        Assert.Equal(first.Result!.Tokenize(), second.Result!.Tokenize());
    }

    [Fact]
    public void Resolve_ResolvesTypeArgumentFromResolutionResult()
    {
        var resolver = new TypeResolver();
        resolver.Initialize(CreateCompilation());
        var stringResult = resolver.Resolve(TypeQuery.FromSpecialType(SpecialType.System_String));
        var listQuery = TypeQuery.FromMetadataName("System.Collections.Generic.List`1", stringResult);

        var result = resolver.Resolve(listQuery);

        Assert.True(result.Success);
        Assert.Empty(result.Diagnostics);
        Assert.Null(result.Exception);
        Assert.Equal("global::System.Collections.Generic.List<string>", result.Result!.Tokenize());
    }

    [Fact]
    public void Resolve_PropagatesDiagnosticsFromUnsuccessfulResolutionTypeArgument()
    {
        var resolver = new TypeResolver();
        resolver.Initialize(CreateCompilation());
        var missingResult = resolver.Resolve(TypeQuery.FromName("DoesNotExist"));
        var listQuery = TypeQuery.FromMetadataName("System.Collections.Generic.List`1", missingResult);

        var result = resolver.Resolve(listQuery);

        Assert.NotEmpty(result.Diagnostics);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic is Emission.Validation.MissingTypeDiagnostic);
    }

    private static CSharpCompilation CreateCompilation() => CSharpCompilation.Create(
        "Resolver",
        references:
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location)
        ]);
}
