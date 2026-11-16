using Emission.Builders;
using Xunit;

namespace Emission.UnitTests.Builders.Enum;

public class EnumBuilderTests
{
    private const string _ns = "Test.Namespace";

    [Fact]
    public void Validate_ReportsMissingName()
    {
        Assert.Single(new EnumBuilder().UseNamespace(_ns).Validate());
    }

    [Fact]
    public void UseValues_ReportsInvalidIdentifier()
    {
        var builder = new EnumBuilder()
            .UseNamespace(_ns)
            .UseName("status value")
            .UseValues(["not ready", "Ready"]);

        var diags = builder.Validate();
        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildEnum);

        Assert.Equal(2, diags.Count());
        Assert.Contains("enum status value", output);
        Assert.Contains("not ready = 0", output);
        Assert.Contains("Ready = 1", output);
    }

    [Fact]
    public void UseValues_AcceptsEnumerable()
    {
        IEnumerable<string> values = new[] { "notready", "Ready" }.Where(value => value.Length > 0);

        var builder = new EnumBuilder()
            .UseNamespace(_ns)
            .UseName("status value")
            .UseValues(values);

        Assert.Equal(["notready", "Ready"], builder.Values);
    }

    [Fact]
    public void UseFlags_BuildsBitShifts()
    {
        var builder = new EnumBuilder()
            .UseNamespace(_ns)
            .UseName("flags")
            .AsFlags()
            .UseValues(["first", "second"]);

        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildEnum);

        Assert.Contains("first = 1 << 0", output);
        Assert.Contains("second = 1 << 1", output);
    }
}
