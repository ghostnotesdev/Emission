using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Base;

public class AssignmentBuilderTests
{
    [Fact]
    public void Validate_ReportsMissingAssignment()
    {
        var builder = new AssignmentBuilder();

        Assert.Single(builder.Validate());
    }

    [Fact]
    public void UseAssignment_StatementBuilderBuildsAnAssignmentWithTerminator()
    {
        var builder = new AssignmentBuilder()
            .UseAssignment(new StatementBuilder().UseStatement("value"));
        var output = TestSupport.CompilationTestHelpers.Build(builder.Build);

        Assert.Equal("= value;", output);
    }

    [Fact]
    public void UseAssignment_IStatementBuilderBuildsAnAssignmentWithTerminator()
    {
        var builder = new AssignmentBuilder()
            .UseAssignment(new StatementBuilder().UseStatement("value"));

        Assert.Equal("= value;", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }

    [Fact]
    public void ConfigureAssignment_CompoundCallbackBuildsCallbackOutput()
    {
        var builder = new AssignmentBuilder()
            .ConfigureAssignment(sb => sb.UseStatement("value;"));

        Assert.Equal("= value;", TestSupport.CompilationTestHelpers.Build(builder.Build));
    }
}
