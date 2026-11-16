namespace Emission.Builders;
public interface IMethodBuilder : IBuilder
{
    bool IsInterfaceMethod { get; }
    string MethodName { get; }
    string ExtensionMethodParameterName { get; }
    IMethodModifierBuilder Modifier { get; }
    IGenericArgumentsBuilder Generics { get; }
    ITypeBuilder? ReturnType { get; }
    IExpressionBuilder? Expression { get; }
    IBodyExpressionBuilder? Body { get; }
    IEnumerable<IMethodParameterBuilder> Parameters { get; }
    bool IsExtensionMethod { get; }
    bool IsValidExtensionMethod { get; }

    IMethodBuilder UseName(string methodName);
    IMethodBuilder ConfigureReturnType(Action<ITypeBuilder> configure);
    IMethodBuilder UseReturnType(ITypeBuilder builder);
    IMethodBuilder ConfigureModifiers(Action<IMethodModifierBuilder> configure);
    IMethodBuilder UseModifiers(IMethodModifierBuilder builder);
    IMethodBuilder ConfigureExpression(Action<IExpressionBuilder> configure);
    IMethodBuilder UseExpression(IExpressionBuilder builder);
    IMethodBuilder ConfigureBodyExpression(Action<IBodyExpressionBuilder> configure);
    IMethodBuilder UseBodyExpression(IBodyExpressionBuilder builder);
    IMethodBuilder UseParameters(IEnumerable<IMethodParameterBuilder> parameters);
    IMethodBuilder WithParameter(Action<IMethodParameterBuilder> configure);
    IMethodBuilder WithParameter(IMethodParameterBuilder builder);
    IMethodBuilder ConfigureGenericArguments(Action<IGenericArgumentsBuilder> configure);
    IMethodBuilder UseGenericArguments(IGenericArgumentsBuilder builder);
    IMethodBuilder UseExtensionMethod(string parameterName);
    IMethodBuilder AsInterfaceMethod();
    void BuildContract(ICompilationStringBuilder icsb);
    void BuildImplementation(ICompilationStringBuilder icsb);
    IStatementParameterBuilder BuildMethodAsParameter();
}
