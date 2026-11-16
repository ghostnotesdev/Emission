namespace Emission.Builders;
public interface IStatementParameterBuilder : ICustomStatementBuilder
{
    ITypeBuilder? ParameterType { get; }
    IStatementBuilder? Statement { get; }
    IStatementParameterBuilder ConfigureType(Action<ITypeBuilder> configure);
    IStatementParameterBuilder UseType(ITypeBuilder builder);
    IStatementParameterBuilder ConfigureStatement(Action<IStatementBuilder> configure);
    IStatementParameterBuilder UseStatement(IStatementBuilder builder);
}
