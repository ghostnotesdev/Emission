using Emission.Validation;

namespace Emission.Builders;
public class ConstructorModifierBuilder : ModifierBuilder<ConstructorModifierBuilder, IConstructorModifierBuilder>, IConstructorModifierBuilder
{
    public override IEnumerable<IDiagnostic> Validate()
    {
        yield break;
    }
}
