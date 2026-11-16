using Emission.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Emission.UnitTests.Context;

public class GeneratorContextTests
{
    [Fact]
    public void CreateBuilder_ReturnsAConfiguredSourceBuilder()
    {
        var context = new GeneratorContext();
        context.Initialize(default, CSharpCompilation.Create("Context"), [], CancellationToken.None);

        var builder = context.CreateBuilder();

        Assert.NotNull(builder);
    }
}
