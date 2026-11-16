using Emission.Builders;
using Emission.Types;
using Xunit;

namespace Emission.UnitTests.Builders.Statements;

public class StatementParameterBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingStatement()
    {
        Assert.Equal(2, new StatementParameterBuilder().Validate().Count());
    }

    [Fact]
    public void ConfigureType_StoresConfiguredType()
    {
        var builder = new StatementParameterBuilder()
            .ConfigureType(type => type.UseType(new TypeReference.Generated("string")));

        Assert.Equal("string", builder.ParameterType!.Reference!.Tokenize());
    }

    [Fact]
    public void UseType_StoresSharedTypeBuilder()
    {
        var type = new TypeBuilder().UseType(new TypeReference.Generated("string"));
        var builder = new StatementParameterBuilder().UseType(type);

        Assert.Same(type, builder.ParameterType);
    }

    [Fact]
    public void UseStatement_BuildsCustomStatement()
    {
        var builder = new StatementParameterBuilder()
            .UseStatement(new StatementBuilder().UseStatement("\"value\""));

        Assert.Equal("\"value\"", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void ConfigureStatement_BuildsConfiguredStatement()
    {
        var builder = new StatementParameterBuilder()
            .ConfigureStatement(statement => statement.UseStatement(
                new ArrayBuilder()
                    .ConfigureType(type => type.UseType(new TypeReference.Generated("int")))
                    .WithEntry(new StatementBuilder().UseStatement("1"))));

        Assert.Contains("new int[]", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void UseStatement_StoresSharedStatementBuilder()
    {
        var statement = new StatementBuilder().UseStatement(
            new ArrayBuilder()
                .ConfigureType(type => type.UseType(new TypeReference.Generated("int"))));
        var builder = new StatementParameterBuilder().UseStatement(statement);

        Assert.Same(statement, builder.Statement);
    }
}
