namespace Emission.Builders;
public interface IConstructorInheritanceBuilder : IBuilder
{
    ITypeBuilder? BaseClassType { get; }
    IEnumerable<IStatementParameterBuilder> Parameters { get; }
    IConstructorInheritanceBuilder ConfigureBaseClassType(Action<ITypeBuilder> configure);
    IConstructorInheritanceBuilder UseBaseClassType(ITypeBuilder builder);
    IConstructorInheritanceBuilder UseParameters(IEnumerable<IStatementParameterBuilder> entries);
    IConstructorInheritanceBuilder WithParameter(Action<IStatementParameterBuilder> configure);
    IConstructorInheritanceBuilder WithParameter(IStatementParameterBuilder builder);
    void Build(ICompilationStringBuilder icsb);
}
