using Emission.Builders;
using Emission.CodeGen.Types;
using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Builders;

public class TypeBuilderExtensionMethodBuilder : MethodBuilder
{
    public TypeBuilderExtensionMethodBuilder(ICodeGenTypeResolver typeResolver, string methodName, INamedTypeSymbol returnType, IParameterSymbol parameterSymbol)
    {
        this.ConfigureReturnType(t => t.UseType(returnType))
            .UseName(methodName)
            .UseExtensionMethod("builder")
            .WithParameter(paramBuilder => paramBuilder
                .ConfigureType(type => type.UseType(parameterSymbol.Type))
                .UseName(parameterSymbol.Name))
            .ConfigureExpression((IExpressionBuilder expression) => expression
                    .ConfigureExpression(statement => statement
                        .UseStatement($"builder.{methodName}(new TypeBuilder().UseType({parameterSymbol.Name}))")));
    }

    public static TypeBuilderExtensionMethodBuilder[] GetAll(ICodeGenTypeResolver typeResolver, string methodName, INamedTypeSymbol returnType)
    {
        IEnumerable<IMethodSymbol> methods = typeResolver.ITypeBuilder
            .GetMembers("UseType")
            .OfType<IMethodSymbol>();

        return [.. methods.Select(method => new TypeBuilderExtensionMethodBuilder(typeResolver, methodName, returnType, method.Parameters.Single()))];
    }
}
