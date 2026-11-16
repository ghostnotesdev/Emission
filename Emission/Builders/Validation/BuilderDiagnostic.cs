using Emission.Validation;
using System.Text;

namespace Emission.Builders;
public class BuilderDiagnostic(IBuilder builder, IDiagnostic orginiatingDiagnostic) : BuilderDiagnosticBase(builder), IBuilderDiagnostic<BuilderDiagnostic>
{
    private readonly Stack<IBuilder> _hierarchy = new();
    public IDiagnostic OriginatingDiagnostic { get; } = orginiatingDiagnostic;
    public override string Message => BuildMessage();

    public BuilderDiagnostic WithParent(IBuilder builder)
    {
        _hierarchy.Push(builder);
        return this;
    }

    public string BuildMessage()
    {
        CompilationStringBuilder sb = new();
        sb.AppendLine(OriginatingDiagnostic.Message)
          .IncreaseIndent()
          .AppendLine("Location: ")
          .IncreaseIndent();

        foreach (IBuilder builder in _hierarchy)
        {
            sb.AppendLine(GetBuilderKey(builder))
              .IncreaseIndent()
              .Append("-> ");
        }

        sb.Append(GetBuilderKey(OriginatingBuilder));
          

        return sb.ToString();
    }
}
