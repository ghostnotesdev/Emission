using Microsoft.CodeAnalysis;

namespace Emission.Types;

public interface ITypeResolver
{
    /// <summary>
    /// Must be called prior to using the type resolver.
    /// This is used instead of a constructor to allow for the use of the new() constraint.
    /// </summary>
    void Initialize(Compilation compilation);

    /// <summary>
    /// Resolves a type using the specified query.
    /// </summary>
    /// <param name="query">The query that defines the type to resolve.</param>
    /// <returns>The result of the type resolution operation.</returns>
    TypeResolutionResult Resolve(TypeQuery query);

    /// <summary>
    /// Resolves the symbol that matches the specified type query.
    /// </summary>
    /// <param name="query">The type query to use when locating a symbol.</param>
    /// <returns>A <see cref="SymbolLookupResult"/> that contains the result of the symbol lookup.</returns>
    SymbolLookupResult ResolveSymbol(TypeQuery query);

    /// <summary>
    /// Resolves a namespace identifier to a namespace resolution result.
    /// </summary>
    /// <param name="_namespace">Namespace name or alias to resolve.</param>
    /// <returns>A <see cref="NamespaceLookupResult"/> containing the outcome of the namespace resolution.</returns>
    NamespaceLookupResult ResolveNamespace(string _namespace);
}
