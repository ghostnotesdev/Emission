using Emission.Builders;
using Emission.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class EnumValueBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingTypeAndValue()
    {
        Assert.Equal(3, new EnumValueBuilder().Validate().Count());
    }

    [Fact]
    public void ConfigureTypeAndUseValue_BuildsQualifiedEnumValue()
    {
        var builder = new EnumValueBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("Status")))
            .UseValue("Ready");

        Assert.Equal("Status.Ready", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("Status"));
        var builder = new EnumValueBuilder().UseType(type);

        Assert.Same(type, builder.EnumType);
    }

    [Fact]
    public void Validate_AcceptsExistingEnumMember()
    {
        var builder = new EnumValueBuilder()
            .UseType(new TypeBuilder().UseType(new TypeReference.Symbol(GetEnumSymbol())))
            .UseValue("Ready");

        Assert.Empty(builder.Validate());
    }

    [Fact]
    public void Validate_ReportsMissingEnumMember()
    {
        var builder = new EnumValueBuilder()
            .UseType(new TypeBuilder().UseType(new TypeReference.Symbol(GetEnumSymbol())))
            .UseValue("Missing");

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("Missing"));
    }

    [Fact]
    public void Validate_ReportsNonEnumSymbol()
    {
        var builder = new EnumValueBuilder()
            .UseType(new TypeBuilder().UseType(new TypeReference.Symbol(GetStringSymbol())))
            .UseValue("Value");

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("does not represent an enum"));
    }

    [Fact]
    public void Validate_ReportsGenericTypeReference()
    {
        var list = GetCompilation().GetTypeByMetadataName("System.Collections.Generic.List`1")!;
        var builder = new EnumValueBuilder()
            .UseType(new TypeBuilder().UseType(new TypeReference.Generic(list, new TypeReference.Generated("string"))))
            .UseValue("Value");

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("cannot represent an enum"));
    }

    [Fact]
    public void Validate_ReportsTupleTypeReference()
    {
        var builder = new EnumValueBuilder()
            .UseType(new TypeBuilder().UseType(new TypeReference.Tuple(new TypeReference.Generated("string"))))
            .UseValue("Value");

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("cannot represent an enum"));
    }

    private static INamedTypeSymbol GetEnumSymbol() =>
        GetCompilation().GetTypeByMetadataName("Status")!;

    private static ITypeSymbol GetStringSymbol() =>
        GetCompilation().GetSpecialType(SpecialType.System_String);

    private static CSharpCompilation GetCompilation() => CSharpCompilation.Create(
        "EnumValueBuilderTests",
        [CSharpSyntaxTree.ParseText("public enum Status { Ready, Done }")],
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location)
        ]);
}
