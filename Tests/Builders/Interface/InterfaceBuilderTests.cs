using Emission.Builders;
using Emission.Types;
using Moq;
using Xunit;

namespace Emission.UnitTests.Builders.Interface;

public class InterfaceBuilderTests
{
    private const string _ns = "Test.Namespace";

    [Fact]
    public void Validate_ReportsMissingName()
    {
        Assert.Contains(new InterfaceBuilder().UseNamespace(_ns).Validate(), diagnostic => diagnostic.Message.Contains("Name"));
    }

    [Fact]
    public void Validate_ReportsInvalidInheritance()
    {
        var builder = new InterfaceBuilder()
            .UseNamespace(_ns)
            .UseName("IThing")
            .ConfigureInheritance(inheritance => inheritance.WithImplementation(_ => { }));

        Assert.Contains(builder.Validate(), diagnostic => diagnostic.Message.Contains("UseType"));
    }

    [Fact]
    public void Build_EmitsPropertyAndMethodContracts()
    {
        var property = new PropertyBuilder()
            .UseName("Value")
            .ConfigureType(type => type.UseType(new TypeReference.Generated("string")))
            .AsInterfaceProperty()
            .ConfigureModifiers(modifier => modifier.UseAccessModifier(AccessModifier.Public));
        var method = new MethodBuilder()
            .UseName("Run")
            .AsInterfaceMethod()
            .ConfigureModifiers(modifier => modifier.UseAccessModifier(AccessModifier.Public));
        var builder = new InterfaceBuilder()
            .UseNamespace(_ns)
            .UseName("IThing")
            .WithProperty(property)
            .WithMethod(method);

        var output = TestSupport.CompilationTestHelpers.Build(builder.BuildInterface);

        Assert.Contains("interface IThing", output);
        Assert.Contains("string Value { get; }", output);
        Assert.Contains("void Run()", output);
        Assert.Contains(";", output);
    }

    [Fact]
    public void FromClassBuilder_ImportsOnlyInterfaceMembers()
    {
        string className = "TestClass";
        var classBuilder = new Mock<IClassBuilder>();

        var classModBuilder = new Mock<IClassModifierBuilder>();
        classModBuilder.SetupGet(mod => mod.AccessModifier).Returns(AccessModifier.Public);

        var interfaceProperty = new Mock<IPropertyBuilder>();
        interfaceProperty.SetupGet(property => property.IsInterfaceProperty).Returns(true);
        var implementationProperty = new Mock<IPropertyBuilder>();

        var interfaceMethod = new Mock<IMethodBuilder>();
        interfaceMethod.SetupGet(method => method.IsInterfaceMethod).Returns(true);

        var implementationMethod = new Mock<IMethodBuilder>();

        var propModBuilder = new Mock<IPropertyModifierBuilder>();
        propModBuilder.SetupGet(b => b.AccessModifier).Returns(AccessModifier.Public);
        interfaceProperty.SetupGet(i => i.Modifier).Returns(propModBuilder.Object);

        var methodModBuilder = new Mock<IMethodModifierBuilder>();
        methodModBuilder.SetupGet(b => b.AccessModifier).Returns(AccessModifier.Public);
        interfaceMethod.SetupGet(i => i.Modifier).Returns(methodModBuilder.Object);

        var inheritanceBuilder = new Mock<IClassInheritanceBuilder>();
        ITypeBuilder? interfaceBuilderRef = null;

        inheritanceBuilder.Setup(i => i.WithImplementation(It.IsAny<ITypeBuilder>()))
            .Returns<ITypeBuilder>(t =>
            {
                interfaceBuilderRef = t;
                return inheritanceBuilder.Object;
            });

        classBuilder.SetupGet(builder => builder.ModifierBuilder).Returns(classModBuilder.Object);
        classBuilder.SetupGet(builder => builder.Name).Returns(className);
        classBuilder.SetupGet(builder => builder.Properties).Returns([interfaceProperty.Object, implementationProperty.Object]);
        classBuilder.SetupGet(builder => builder.Methods).Returns([interfaceMethod.Object, implementationMethod.Object]);

        classBuilder.Setup(b => b.ConfigureInheritance(It.IsAny<Action<IClassInheritanceBuilder>>()))
            .Returns<Action<IClassInheritanceBuilder>>(configure =>
            {
                configure(inheritanceBuilder.Object);
                return classBuilder.Object;
            })
            .Verifiable();

        var builder = InterfaceBuilder.FromClassBuilder(classBuilder.Object);

        Assert.NotNull(interfaceBuilderRef);
        Assert.Same(builder, interfaceBuilderRef);
        Assert.Equal("I" + className, builder.Name);
        Assert.Equal(classModBuilder.Object.AccessModifier, builder.ModifierBuilder.AccessModifier);
        Assert.Same(interfaceProperty.Object, Assert.Single(builder.Properties));
        Assert.Same(interfaceMethod.Object, Assert.Single(builder.Methods));
    }
}
