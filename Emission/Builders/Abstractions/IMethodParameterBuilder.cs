namespace Emission.Builders;
public interface IMethodParameterBuilder : IBuilder
{
    string ParameterName { get; }
    ITypeBuilder ParameterType { get; }
    IMethodParameterBuilder UseName(string propertyName);
    IMethodParameterBuilder ConfigureType(Action<ITypeBuilder> configure);
    IMethodParameterBuilder UseType(ITypeBuilder builder);
    void Build(ICompilationStringBuilder icsb);
}
