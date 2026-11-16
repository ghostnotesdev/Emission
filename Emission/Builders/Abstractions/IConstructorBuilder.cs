namespace Emission.Builders;
public interface IConstructorBuilder : IBuilder
{
    IConstructorModifierBuilder Modifier { get; }
    IExpressionBuilder? Expression { get; }
    IBodyExpressionBuilder? Body { get; }
    IEnumerable<IMethodParameterBuilder> Parameters { get; }
    IConstructorInheritanceBuilder? BaseConstructor { get; }

    IConstructorBuilder ConfigureModifiers(Action<IConstructorModifierBuilder> configure);
    IConstructorBuilder UseModifiers(IConstructorModifierBuilder builder);
    IConstructorBuilder ConfigureExpression(Action<IExpressionBuilder> configure);
    IConstructorBuilder UseExpression(IExpressionBuilder builder);
    IConstructorBuilder ConfigureBodyExpression(Action<IBodyExpressionBuilder> configure);
    IConstructorBuilder UseBodyExpression(IBodyExpressionBuilder builder);
    IConstructorBuilder UseParameters(IEnumerable<IMethodParameterBuilder> builders);
    IConstructorBuilder WithParameter(Action<IMethodParameterBuilder> configure);
    IConstructorBuilder WithParameter(IMethodParameterBuilder builder);
    IConstructorBuilder ConfigureBaseConstructor(Action<IConstructorInheritanceBuilder> configure);
    IConstructorBuilder UseBaseConstructor(IConstructorInheritanceBuilder builder);
    void Build(ICompilationStringBuilder icsb, string className);
}
