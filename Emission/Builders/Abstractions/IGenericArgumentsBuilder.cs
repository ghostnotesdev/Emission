namespace Emission.Builders;
public interface IGenericArgumentsBuilder : IBuilder
{
    IEnumerable<IGenericArgumentBuilder> Arguments { get; }
    IGenericArgumentsBuilder UseArguments(IEnumerable<IGenericArgumentBuilder> builders);
    IGenericArgumentsBuilder WithArgument(Action<IGenericArgumentBuilder> configure);
    IGenericArgumentsBuilder WithArgument(IGenericArgumentBuilder builder);
    void BuildDeclaration(ICompilationStringBuilder icsb);
    void BuildConstraints(ICompilationStringBuilder icsb);
}
