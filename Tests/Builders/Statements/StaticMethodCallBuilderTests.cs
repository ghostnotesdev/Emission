using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class StaticMethodCallBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingClassAndMethod()
    {
        Assert.Equal(2, new StaticMethodCallBuilder().Validate().Count());
    }

    [Fact]
    public void OnClassAndCallMethod_BuildsCallWithArguments()
    {
        var builder = new StaticMethodCallBuilder()
            .OnClass(type => type.UseType(new TypeReference.Generated("Console")))
            .CallMethod("WriteLine")
            .WithParameter(p => p
                .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
                .UseStatement(new StatementBuilder().UseStatement("\"hello\"")));

        Assert.Equal("Console.WriteLine(\"hello\")", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseParameters_AcceptsEnumerable()
    {
        IEnumerable<IStatementParameterBuilder> parameters =
        [
            new StatementParameterBuilder()
                .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
                .UseStatement(new StatementBuilder().UseStatement("\"hello\""))
        ];

        var builder = new StaticMethodCallBuilder()
            .UseParameters(parameters);

        Assert.Single(builder.Parameters);
    }

    [Fact]
    public void OnClass_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("Console"));
        var builder = new StaticMethodCallBuilder();

        builder.OnClass(type).CallMethod("WriteLine");

        Assert.Same(type, builder.Class);
        Assert.Equal("Console.WriteLine()", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
