using Emission.Extensions;
using Emission.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Extensions;

public class SymbolExtensionsTests
{
    [Fact]
    public void GenericTypeReference_AcceptsEnumerableTypeParameters()
    {
        var list = CreateCompilation().GetTypeByMetadataName("System.Collections.Generic.List`1")!;
        IEnumerable<TypeReference> typeParameters = [new TypeReference.Generated("string")];

        var result = list.GenericTypeReference(typeParameters);

        Assert.Equal("global::System.Collections.Generic.List<string>", result.Tokenize());
    }

    [Fact]
    public void SingleMatchOrDefault_AcceptsEnumerableSignature()
    {
        var type = CreateCompilation()
            .GetTypeByMetadataName("TestType")!;
        var method = type.GetMembers("Method").OfType<IMethodSymbol>().Single();
        IEnumerable<TypeReference> signature = [method.Parameters[0].Type.TypeReference()];

        var result = new[] { method }.SingleMatchOrDefault(signature);

        Assert.Same(method, result);
    }

    [Fact]
    public void SingleMatchOrDefaultByName_AcceptsEnumerableSignature()
    {
        var type = CreateCompilation()
            .GetTypeByMetadataName("TestType")!;
        var method = type.GetMembers("Method").OfType<IMethodSymbol>().Single();
        IEnumerable<TypeReference> signature = [method.Parameters[0].Type.TypeReference()];

        var result = new[] { method }.SingleMatchOrDefault("Method", signature);

        Assert.Same(method, result);
    }

    private static CSharpCompilation CreateCompilation() =>
        CSharpCompilation.Create(
            "Extensions",
            [
                CSharpSyntaxTree.ParseText("public class TestType { public void Method(string value) { } }")
            ],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            ]);
}
