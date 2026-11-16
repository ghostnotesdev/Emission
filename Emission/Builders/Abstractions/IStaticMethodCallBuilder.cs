
namespace Emission.Builders;
public interface IStaticMethodCallBuilder : ICustomStatementBuilder
{
    IGenericArgumentsBuilder Generics { get; }
    ITypeBuilder Class { get; }
    string Method { get; }
    IEnumerable<IStatementParameterBuilder> Parameters { get; }
    IStaticMethodCallBuilder CallMethod(string methodName);
    IStaticMethodCallBuilder OnClass(Action<ITypeBuilder> configure);
    IStaticMethodCallBuilder OnClass(ITypeBuilder builder);
    IStaticMethodCallBuilder UseParameters(IEnumerable<IStatementParameterBuilder> parameterMappings);
    IStaticMethodCallBuilder WithParameter(Action<IStatementParameterBuilder> configure);
    IStaticMethodCallBuilder WithParameter(IStatementParameterBuilder builder);
    IStaticMethodCallBuilder ConfigureGenericArguments(Action<IGenericArgumentsBuilder> configure);
    IStaticMethodCallBuilder UseGenericArguments(IGenericArgumentsBuilder builder);
}
