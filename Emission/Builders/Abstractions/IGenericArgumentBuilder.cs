using System.Xml.Linq;

namespace Emission.Builders;
public interface IGenericArgumentBuilder : IBuilder
{
    string ArgumentName { get; }
    IGenericConstraintBuilder Constraints { get; }
    IGenericArgumentBuilder UseName(string name);
    IGenericArgumentBuilder ConfigureConstraints(Action<IGenericConstraintBuilder> configure);
    IGenericArgumentBuilder UseConstraints(IGenericConstraintBuilder builder);
    void BuildDeclaration(ICompilationStringBuilder icsb);
    void BuildConstraints(ICompilationStringBuilder icsb);
}
