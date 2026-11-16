using Emission.Builders;
using Moq;
using Xunit;

namespace Emission.UnitTests.Builders.Source;

public class SourceBuilderTests
{
    [Fact]
    public void Validate_ReportsEmptyNamespaceAndHintName()
    {
        var builder = new SourceBuilder(new CompilationStringBuilder());

        Assert.Equal(3, builder.Validate().Count());
    }

    [Fact]
    public void Build_UsesInjectedChildAndCachesUntilConfigurationChanges()
    {
        var classBuilder = new Mock<IClassBuilder>();
        classBuilder.Setup(child => child.Validate()).Returns([]);
        classBuilder.Setup(child => child.BuildClass(It.IsAny<ICompilationStringBuilder>()))
            .Callback<ICompilationStringBuilder>(builder => builder.AppendLine("class Generated {}"));

        var source = new SourceBuilder()
            .UseNamespace("Generated")
            .UseHintName("Generated")
            .WithClass(classBuilder.Object);

        var first = source.Build();
        var second = source.Build();

        Assert.Equal(new("Generated", first.Source), first);
        Assert.Equal(first.Source, second.Source);
        classBuilder.Verify(child => child.BuildClass(It.IsAny<ICompilationStringBuilder>()), Times.Once);
    }

    [Fact]
    public void Build_RejectsInvalidConfiguration()
    {
        var source = new SourceBuilder()
            .UseNamespace("Generated")
            .UseHintName("Generated");

        Assert.Throws<InvalidOperationException>(() => source.Build());
    }
}
