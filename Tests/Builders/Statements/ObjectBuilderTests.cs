using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class ObjectBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingType()
    {
        Assert.Single(new ObjectBuilder().Validate());
    }

    [Fact]
    public void UseParameters_BuildsObjectCreation()
    {
        var builder = new ObjectBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("Person")))
            .WithParameter(p => p
                .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
                .UseStatement(new StatementBuilder().UseStatement("\"Ada\"")));

        Assert.Equal("new Person(\"Ada\")", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseParameters_AcceptsEnumerable()
    {
        IEnumerable<IStatementParameterBuilder> parameters =
        [
            new StatementParameterBuilder()
                .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
                .UseStatement(new StatementBuilder().UseStatement("\"Ada\""))
        ];

        var builder = new ObjectBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("Person")))
            .UseParameters(parameters);

        Assert.Single(builder.Parameters);
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("Person"));
        var builder = new ObjectBuilder().UseType(type);

        Assert.Same(type, builder.Type);
    }
}
