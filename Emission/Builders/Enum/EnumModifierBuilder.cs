using Emission.Validation;

namespace Emission.Builders;
public class EnumModifierBuilder : ModifierBuilder<EnumModifierBuilder, IEnumModifierBuilder>, IEnumModifierBuilder
{
    public override IEnumerable<IDiagnostic> Validate()
    {
        yield break;
    }
}
