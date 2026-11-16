namespace Emission.Builders;
public interface IObjectBuilder : ICustomStatementBuilder
{
    ITypeBuilder Type { get; }
    IEnumerable<IStatementParameterBuilder> Parameters { get; }
    IObjectBuilder ConfigureType(Action<ITypeBuilder> configure);
    IObjectBuilder UseType(ITypeBuilder builder);
    IObjectBuilder UseParameters(IEnumerable<IStatementParameterBuilder> entries);
    IObjectBuilder WithParameter(Action<IStatementParameterBuilder> configure);
    IObjectBuilder WithParameter(IStatementParameterBuilder builder);
}
