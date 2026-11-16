using Emission.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Types;

public class TypeReferenceTests
{
    [Fact]
    public void Generated_TokenizesAndDisplaysItsToken()
    {
        var type = new TypeReference.Generated("global::System.String");

        Assert.Equal("global::System.String", type.Tokenize());
        Assert.Equal("System.String", type.ToDisplayString());
    }

    [Fact]
    public void Generic_TokenizesTypeArguments()
    {
        var symbol = GetCompilation().GetTypeByMetadataName("System.Collections.Generic.List`1")!;
        var type = new TypeReference.Generic(symbol, new TypeReference.Generated("string"));

        Assert.Equal("global::System.Collections.Generic.List<string>", type.Tokenize());
    }

    [Fact]
    public void Tuple_TokenizesElements()
    {
        var type = new TypeReference.Tuple(new TypeReference.Generated("int"), new TypeReference.Generated("string"));

        Assert.Equal("(int, string)", type.ToString());
    }

    [Fact]
    public void Generic_AcceptsEnumerableTypeParameters()
    {
        var symbol = GetCompilation().GetTypeByMetadataName("System.Collections.Generic.List`1")!;
        IEnumerable<TypeReference> typeParameters = [new TypeReference.Generated("string")];

        var type = new TypeReference.Generic(symbol, typeParameters);

        Assert.Equal("global::System.Collections.Generic.List<string>", type.Tokenize());
    }

    [Fact]
    public void Tuple_AcceptsEnumerableElements()
    {
        IEnumerable<TypeReference> elements = [new TypeReference.Generated("int"), new TypeReference.Generated("string")];

        var type = new TypeReference.Tuple(elements);

        Assert.Equal("(int, string)", type.Tokenize());
    }

    private static CSharpCompilation GetCompilation() => CSharpCompilation.Create(
        "Types",
        references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
}
