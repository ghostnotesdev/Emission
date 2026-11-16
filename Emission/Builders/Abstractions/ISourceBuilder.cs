using Emission.Validation;

namespace Emission.Builders;
public interface ISourceBuilder
{
    string Namespace { get; }
    string HintName { get; }
    IEnumerable<IEnumBuilder> Enums { get; }
    IEnumerable<IInterfaceBuilder> Interfaces { get; }
    IEnumerable<IClassBuilder> Classes { get; }

    ISourceBuilder UseNamespace(string ns);
    ISourceBuilder UseHintName(string hintName);
    ISourceBuilder UseEnums(IEnumerable<IEnumBuilder> builders);
    ISourceBuilder WithEnum(Action<IEnumBuilder> configure);
    ISourceBuilder WithEnum(IEnumBuilder builder);
    ISourceBuilder UseInterfaces(IEnumerable<IInterfaceBuilder> builders);
    ISourceBuilder WithInterface(Action<IInterfaceBuilder> configure);
    ISourceBuilder WithInterface(IInterfaceBuilder builder);
    ISourceBuilder UseClasses(IEnumerable<IClassBuilder> builders);
    ISourceBuilder WithClass(Action<IClassBuilder> configure);
    ISourceBuilder WithClass(IClassBuilder builder);
    IEnumerable<IDiagnostic> Validate();
    BuiltSource Build();
}
