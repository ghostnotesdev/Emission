using Microsoft.CodeAnalysis;
using Emission.Types;

namespace Emission.Extensions;

public static class SymbolExtensions
{
    // Centralizes symbol tokenization logic.
    // The resulting token is what we use to write the source value for the type in generated code.
    public static readonly SymbolDisplayFormat TokenFormat = SymbolDisplayFormat.FullyQualifiedFormat;
    public static string Tokenize(this ITypeSymbol symbol) => symbol.ToDisplayString(TokenFormat);

    public static TypeReference TypeReference(this ITypeSymbol type) => new TypeReference.Symbol(type);
    public static TypeReference GenericTypeReference(this INamedTypeSymbol type, params TypeReference[] typeParameters) =>
        GenericTypeReference(type, (IEnumerable<TypeReference>)typeParameters);

    public static TypeReference GenericTypeReference(this INamedTypeSymbol type, IEnumerable<TypeReference> typeParameters) =>
        new TypeReference.Generic(type, typeParameters);

    public static TypeReference ArrayTypeReference(this IArrayTypeSymbol array, params TypeReference[] typeParameters) =>
        ArrayTypeReference(array, (IEnumerable<TypeReference>)typeParameters);

    public static TypeReference ArrayTypeReference(this IArrayTypeSymbol array, IEnumerable<TypeReference> typeParameters) =>
        new TypeReference.Array(GetArrayTypeReference(array.ElementType, typeParameters), array.Rank);

    private static TypeReference GetArrayTypeReference(ITypeSymbol elementType, IEnumerable<TypeReference> typeParameters)
    {
        if (elementType is INamedTypeSymbol namedSymbol && typeParameters.Any())
        {
            return namedSymbol.GenericTypeReference(typeParameters);
        }

        return elementType.TypeReference();
    }

    public static string TokenizeGenericType(this ITypeSymbol symbol)
    {
        string type = symbol.Name;

        if (symbol.ContainingType is not null)
            return $"{symbol.ContainingType.TokenizeGenericType()}.{type}";

        if (!symbol.ContainingNamespace.IsGlobalNamespace)
            return $"global::{symbol.ContainingNamespace.ToDisplayString()}.{type}";

        return $"global::{type}";
    }

    /// <summary>
    /// Verifies a public method constructor matches the provided signature or a single public parameterless constructor exists if signature is not provided.
    /// Only meant to be called on <see cref="INamedTypeSymbol.InstanceConstructors"/>. Calling on any other <see cref="IEnumerable{IMethodSymbol}"/> could produce undesirable results.
    /// </summary>
    /// <param name="methods"></param>
    /// <param name="signature"></param>
    /// <returns></returns>
    public static IMethodSymbol? SingleMatchOrDefault(this IEnumerable<IMethodSymbol> methods, params TypeReference[]? signature)
        => methods.SingleMatchOrDefault((IEnumerable<TypeReference>?)signature);

    public static IMethodSymbol? SingleMatchOrDefault(this IEnumerable<IMethodSymbol> methods, IEnumerable<TypeReference>? signature)
    {
        TypeReference[] signatureArray = [.. signature ?? []];

        return methods
            .Where(m => m.Parameters.Length == signatureArray.Length)
            .Select(m => new
            {
                Method = m,
                Matches = m.Parameters
                    .Zip(signatureArray, (actual, expected) => expected.Matches(actual.Type))
                    .All(matches => matches == true)
            })
            .SingleOrDefault(x => x.Matches)?.Method;
    }

    /// <summary>
    /// Verifies a public method matches the provided signature or a single public parameterless method exists if signature is not provided.
    /// </summary>
    /// <param name="methods"></param>
    /// <param name="signature"></param>
    /// <returns></returns>
    public static IMethodSymbol? SingleMatchOrDefault(this IEnumerable<IMethodSymbol> methods, string methodName, params TypeReference[]? signature)
        => methods.SingleMatchOrDefault(methodName, (IEnumerable<TypeReference>?)signature);

    public static IMethodSymbol? SingleMatchOrDefault(this IEnumerable<IMethodSymbol> methods, string methodName, IEnumerable<TypeReference>? signature)
    {
        TypeReference[] signatureArray = [.. signature ?? []];

        return methods
            .Where(m => m.Name == methodName && m.Parameters.Length == signatureArray.Length)
            .Select(ctr => new
            {
                Method = ctr,
                Matches = ctr.Parameters
                    .Zip(signatureArray, (actual, expected) => expected.Matches(actual.Type))
                    .All(matches => matches == true)
            })
            .SingleOrDefault(x => x.Matches)?.Method;
    }

    public static bool Matches(this TypeReference expected, ITypeSymbol actual)
    {
        return expected switch
        {
            TypeReference.Symbol symbol => SymbolEqualityComparer.Default.Equals(symbol.Type, actual),

            TypeReference.Generic generic => actual is INamedTypeSymbol namedType
                && SymbolEqualityComparer.Default.Equals(generic.GenericType.OriginalDefinition, namedType.OriginalDefinition)
                && generic.TypeParameters.Length == namedType.TypeArguments.Length
                && generic.TypeParameters
                    .Zip(namedType.TypeArguments, (expectedArg, actualArg) => expectedArg.Matches(actualArg))
                    .All(matches => matches),

            TypeReference.Generated generated => actual.Tokenize() == generated.Token,

            TypeReference.Tuple tuple => actual is INamedTypeSymbol namedType
                && namedType.IsTupleType
                && tuple.Elements.Length == namedType.TupleElements.Length
                && tuple.Elements
                    .Zip(namedType.TupleElements, (expectedElement, actualElement) => expectedElement.Matches(actualElement.Type))
                    .All(matches => matches),

            TypeReference.Array array => actual is IArrayTypeSymbol arrayType
                && array.ElementType.Matches(arrayType.ElementType)
                && arrayType.Rank == array.Rank,

            _ => false
        };
    }
}
