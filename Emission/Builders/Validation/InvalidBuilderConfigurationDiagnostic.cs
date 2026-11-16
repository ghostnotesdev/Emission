namespace Emission.Builders;
public class InvalidBuilderConfigurationDiagnostic(IBuilder builder, string message) : BuilderDiagnosticBase(builder)
{
    public override string Message => message;
}
