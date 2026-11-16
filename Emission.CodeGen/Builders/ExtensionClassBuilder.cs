using Emission.Builders;

namespace Emission.CodeGen.Builders;

public class ExtensionClassBuilder(string builderInterfaceName) : ClassBuilder
{
    public override string Namespace { get; } = "Emission.Builders";

    public override string Name { get; } = builderInterfaceName + "Extensions";

    public override IClassModifierBuilder ModifierBuilder { get; } = new ClassModifierBuilder()
        .UseAccessModifier(AccessModifier.Public)
        .UseClassModifier(ClassModifier.Static);
}
