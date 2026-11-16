using Emission.Builders;
using Moq;
using Xunit;

namespace Emission.UnitTests.Builders.Class;

public class ClassBuilderTests
{
    private const string _ns = "Test.Namespace";
    private const string _cn = "TestClass";

    [Fact]
    public void Validate_ReportsEmptyName()
    {
        Assert.Contains(new ClassBuilder()
            .UseNamespace(_ns)
            .Validate(), diagnostic => diagnostic.Message.Contains("Name"));
    }

    [Fact]
    public void WithField_UsesInjectedBuilderAndBuildsNestedOutput()
    {
        var field = new Mock<IFieldBuilder>();
        field.Setup(builder => builder.Validate()).Returns([]);
        field.Setup(builder => builder.Build(It.IsAny<ICompilationStringBuilder>()))
            .Callback<ICompilationStringBuilder>(builder => builder.AppendLine("field"));

        var classBuilder = new ClassBuilder()
            .UseNamespace(_ns)
            .UseName(_cn)
            .WithField(field.Object);

        Assert.Same(field.Object, Assert.Single(classBuilder.Fields));
        var output = TestSupport.CompilationTestHelpers.Build(classBuilder.BuildClass);

        Assert.Contains("class TestClass", output);
        Assert.Contains("field", output);
        field.Verify(builder => builder.Build(It.IsAny<ICompilationStringBuilder>()), Times.Once);
    }

    [Fact]
    public void ConfigureGenericArguments_BuildsDeclarationAndConstraints()
    {
        var builder = new ClassBuilder()
            .UseNamespace(_ns)
            .UseName(_cn)
            .ConfigureGenericArguments(arguments => arguments
                .WithArgument(constraint => constraint
                    .UseName("T")
                    .ConfigureConstraints(c => c.UseConstraint(GenericPrimaryConstraint.Class))));

        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildClass);

        Assert.Contains("class TestClass<T>", output);
        Assert.Contains("where T : class", output);
    }
}
