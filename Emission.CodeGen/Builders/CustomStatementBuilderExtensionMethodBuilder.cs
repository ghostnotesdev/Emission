using Emission.Builders;
using Emission.CodeGen.Types;
using Emission.Extensions;
using Emission.Types;
using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Builders;

public class CustomStatementBuilderExtensionMethodBuilder : MethodBuilder
{
    
    public CustomStatementBuilderExtensionMethodBuilder(
        ICodeGenTypeResolver typeResolver,
        INamedTypeSymbol returnType,
        INamedTypeSymbol customStatementBuilder,
        string? generatedConfigureExtensionName = null)
    {
        string implementationName = customStatementBuilder.Name[1..];
        SymbolLookupResult implementationLookup = typeResolver.ResolveSymbol(
            TypeQuery.FromMetadataName(string.Join(".", KnownTypes.BuildersNamespace, implementationName)));
        if (!implementationLookup.Success)
            throw implementationLookup.Exception;
        string extensionName = "Configure" + implementationName.Replace("Builder", "");

        this.ConfigureReturnType(t => t.UseType(returnType))
            .UseName(extensionName)
            .UseExtensionMethod("builder")
            .WithParameter(paramBuilder => paramBuilder
                .ConfigureType(type => type
                    .UseType(typeResolver.ActionT(customStatementBuilder.TypeReference())))
                .UseName("configure"));

        if (generatedConfigureExtensionName is null)
        {
            // IStatementBuilderExtensions.g.cs base method
            ConfigureBodyExpression((IBodyExpressionBuilder body) => body
                .WithStatement((IStatementBuilder statement) => statement.UseStatement("var nested = new " + implementationLookup.Result.Tokenize() + "();"))
                .WithStatement((IStatementBuilder statement) => statement.UseStatement("configure(nested);"))
                .WithStatement((IStatementBuilder statement) => statement.UseStatement("return builder.UseStatement(nested);")));
        }
        else
        {
            // Consumers of IStatementBuilder that call IStatementBuilderExtensions.g.cs methods
            ConfigureExpression(expression => expression
                .ConfigureExpression(statement => statement
                    .UseStatement($"builder.{generatedConfigureExtensionName}(new StatementBuilder().{extensionName}(configure))")));
        }
    }

    public static CustomStatementBuilderExtensionMethodBuilder[] GetAll(ICodeGenTypeResolver typeResolver, INamedTypeSymbol returnType, string? generatedConfigureExtensionName = null)
    {
        return [.. typeResolver
            .GetAllCustomStatementBuilders()
            .Select(b => new CustomStatementBuilderExtensionMethodBuilder(typeResolver, returnType, b, generatedConfigureExtensionName))];
    }
}
