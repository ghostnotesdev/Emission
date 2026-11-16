using Emission.Builders;
using Emission.CodeGen.Types;
using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Builders;

public class StatementBuilderExtensionMethodBuilder : MethodBuilder
{
    public StatementBuilderExtensionMethodBuilder(INamedTypeSymbol returnType, string methodName, IParameterSymbol parameterSymbol)
    {
        this.ConfigureReturnType(t => t.UseType(returnType))
            .UseName(methodName)
            .UseExtensionMethod("builder")
            .WithParameter(paramBuilder => paramBuilder
                .ConfigureType(t => t.UseType(parameterSymbol.Type))
                .UseName(parameterSymbol.Name))
            .ConfigureExpression(expression => expression
                .ConfigureExpression(statement => statement
                    .UseStatement($"builder.{methodName}(new StatementBuilder().UseStatement({parameterSymbol.Name}))")));
    }

    public static StatementBuilderExtensionMethodBuilder[] GetAll(ICodeGenTypeResolver typeResolver, INamedTypeSymbol returnType, string methodName)
    {
        IEnumerable<IMethodSymbol> methods = typeResolver.IStatementBuilder
            .GetMembers("UseStatement")
            .OfType<IMethodSymbol>();

        return [.. methods.Select(method => new StatementBuilderExtensionMethodBuilder(returnType, methodName, method.Parameters.Single()))];
    }
}
