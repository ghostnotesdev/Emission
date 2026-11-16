using Emission.Extensions;
using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Types;

public class TypeResolver : ITypeResolver
{
    private Compilation? _compilation;
    private readonly Dictionary<string, SymbolLookupResult> _cache = [];

    protected Compilation Compilation => _compilation ?? throw new InvalidOperationException($"{nameof(Initialize)} must be called before using {nameof(ITypeResolver)}");

    public void Initialize(Compilation compilation)
    {
        _compilation = compilation;
    }

    public TypeResolutionResult Resolve(TypeQuery query)
    {
        if (_cache.TryGetValue(query.ToString(), out SymbolLookupResult cached))
        {
            return GetTypeResolutionResult(query, cached);
        }

        SymbolLookupResult result = GetAndCacheSymbol(query);
        return GetTypeResolutionResult(query, result);
    }

    public SymbolLookupResult ResolveSymbol(TypeQuery query)
    {
        if (_cache.TryGetValue(query.ToString(), out SymbolLookupResult cached))
        {
            return GetSymbolLookupResult(query, cached);
        }

        SymbolLookupResult result = GetAndCacheSymbol(query);
        return GetSymbolLookupResult(query, result);
    }

    public NamespaceLookupResult ResolveNamespace(string _namespace)
    {
        INamespaceSymbol? symbol = null;
        List<IDiagnostic> diagnostics = [];
        IEnumerable<INamespaceSymbol> currentNamespaces = Compilation
            .GlobalNamespace
            .GetNamespaceMembers();

        string[] namespaceParts = _namespace.Split('.');
        for (int i = 0; i < namespaceParts.Length; i++)
        {
            string part = namespaceParts[i];
            symbol = currentNamespaces.FirstOrDefault(x => x.Name == namespaceParts[i]);
            if (symbol is default(INamespaceSymbol))
            {
                string resolved = i == 0 ? Compilation.GlobalNamespace.Name : string.Join(".", namespaceParts.Take(i));
                diagnostics.Add(new NamespaceLookupDiagnostic(_namespace, $"Failed to find '{part}' in '{resolved}'."));
                break;
            }

            if (i + 1 < namespaceParts.Length)
                currentNamespaces = symbol.GetNamespaceMembers();
        }

        return new(_namespace, symbol, diagnostics);
    }

    private SymbolLookupResult GetAndCacheSymbol(TypeQuery query)
    {
        SymbolLookupResult result = query.Mode switch
        {
            TypeQuery.QueryMode.NameSearch => GetSymbolByNameLookup(query),
            TypeQuery.QueryMode.MetadataReference => GetSymbolByMetadataName(query),
            TypeQuery.QueryMode.SpecialType => GetSymbolBySpecialType(query),
            _ => throw new IndexOutOfRangeException("Invalid type query mode"),
        };
        _cache.Add(query.ToString(), result);
        return result;
    }

    private TypeReference GetTypeReferenceForSymbol(ITypeSymbol symbol, IEnumerable<TypeReference> typeArguments)
    {
        if (typeArguments.Any())
        {
            return symbol switch
            {
                INamedTypeSymbol named when named.IsGenericType =>
                    named.ConstructUnboundGenericType().GenericTypeReference(typeArguments),

                IArrayTypeSymbol array =>
                    Compilation.CreateArrayTypeSymbol(array.ElementType, array.Rank).ArrayTypeReference(typeArguments),

                _ => symbol.TypeReference()
            };
        }

        return symbol.TypeReference();
    }

    private SymbolLookupResult GetSymbolByNameLookup(TypeQuery query)
    {
        List<INamedTypeSymbol> matches = [.. Compilation
            .GetSymbolsWithName(query.Name, SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .Where(s => s.Arity == query.TypeArguments.Length)];

        if (matches.Count == 0)
            return new(query, null, [new MissingTypeDiagnostic(query.Name, query.TypeArguments.Length)]);

        if (matches.Count > 1)
            return new(query, null, [new AmbiguousTypeDiagnostic(query.Name, query.TypeArguments.Length, matches)]);

        return GetSymbolLookupResult(query, matches[0]);
    }

    private SymbolLookupResult GetSymbolByMetadataName(TypeQuery query)
    {
        INamedTypeSymbol? symbol = Compilation.GetTypeByMetadataName(query.MetadataName);
        if (symbol is null)
            return new(query, null, [new MissingTypeDiagnostic(query.MetadataName)]);

        return GetSymbolLookupResult(query, symbol);
    }

    private SymbolLookupResult GetSymbolBySpecialType(TypeQuery query)
    {
        return GetSymbolLookupResult(query, Compilation.GetSpecialType(query.SpecialType));
    }

    private SymbolLookupResult GetSymbolLookupResult(TypeQuery query, INamedTypeSymbol symbol)
    {
        ITypeSymbol result;
        if (query.IsArray)
        {
            result = Compilation.CreateArrayTypeSymbol(symbol, query.ArrayRank);
        }
        else
            result = symbol;

        return new(query, result, null);
    }

    private TypeResolutionResult GetTypeResolutionResult(TypeQuery query, SymbolLookupResult symbolResult)
    {
        List<IDiagnostic> diagnostics = [];
        if (symbolResult.Success)
        {
            List<TypeReference> resolvedArgs = [];
            foreach (var arg in query.TypeArguments)
            {
                switch (arg)
                {
                    case TypeQueryArgument.Query argQuery:
                        TypeResolutionResult argResult = Resolve(argQuery.Value);
                        if (argResult.Success)
                            resolvedArgs.Add(argResult.Result);
                        else
                            diagnostics.AddRange(argResult.Diagnostics);
                        break;
                    case TypeQueryArgument.Reference reference:
                        if (reference.Value is null)
                            diagnostics.Add(new MissingTypeDiagnostic(query.ToString(), query.TypeArguments.Count()));
                        else
                            resolvedArgs.Add(reference.Value);
                        break;
                    case TypeQueryArgument.Resolution resolution:
                        if (resolution.Value.Success)
                            resolvedArgs.Add(resolution.Value.Result);
                        else
                            diagnostics.AddRange(resolution.Value.Diagnostics);
                        break;
                }
            }

            var typeRef = GetTypeReferenceForSymbol(symbolResult.Result, resolvedArgs);
            return new(query, typeRef, diagnostics);
        }
        else
        {
            diagnostics.AddRange(symbolResult.Diagnostics);
        }
        return new(query, null, diagnostics);
    }

    private SymbolLookupResult GetSymbolLookupResult(TypeQuery query, SymbolLookupResult symbolResult)
    {
        if (!symbolResult.Success || query.TypeArguments.Length == 0)
            return symbolResult;

        if (symbolResult.Result is not INamedTypeSymbol namedSymbol)
            return new(query, null, [new SymbolLookupDiagnostic(query, "Symbol must be extend INamedTypeSymbol in order to have generic arguments")]);

        List<IDiagnostic> diagnostics = [];
        List<ITypeSymbol> arguments = [];
        foreach (TypeQueryArgument arg in query.TypeArguments)
        {
            switch (arg)
            {
                case TypeQueryArgument.Query argQuery:
                    SymbolLookupResult argResult = ResolveSymbol(argQuery.Value);
                    if (!argResult.Success)
                        diagnostics.AddRange(argResult.Diagnostics);
                    else
                        arguments.Add(argResult.Result);
                    break;
                case TypeQueryArgument.Reference reference:
                    if (reference.Value is null)
                        diagnostics.Add(new MissingTypeDiagnostic(query.ToString(), query.TypeArguments.Count()));
                    else if (!TryGetSymbol(reference.Value, out ITypeSymbol? referenceSymbol))
                        diagnostics.Add(new SymbolLookupDiagnostic(query, "Failed to parse symbol from TypeReference: " + reference.ToString()));
                    else
                        arguments.Add(referenceSymbol!);
                    break;
                case TypeQueryArgument.Resolution resolution:
                    if (!resolution.Value.Success)
                        diagnostics.AddRange(resolution.Value.Diagnostics);
                    else if (!TryGetSymbol(resolution.Value.Result, out ITypeSymbol? resolutionSymbol))
                        diagnostics.Add(new SymbolLookupDiagnostic(query, "Failed to parse symbol from TypeReference: " + resolution.Value.Result.ToString()));
                    else
                        arguments.Add(resolutionSymbol!);
                    break;
            }
        }

        if (diagnostics.Count == 0)
        {
            INamedTypeSymbol constructed = namedSymbol.Construct([.. arguments]);
            return new SymbolLookupResult(query, constructed, null);
        }

        return new SymbolLookupResult(query, null, diagnostics);
    }

    private bool TryGetSymbol(TypeReference reference, out ITypeSymbol? result)
    {
        switch (reference)
        {
            case TypeReference.Symbol symbol:
                result = symbol.Type;
                return true;

            case TypeReference.Generic generic:
                List<ITypeSymbol> args = [];
                foreach (var parameter in generic.TypeParameters)
                {
                    if (!TryGetSymbol(parameter, out ITypeSymbol? argResult))
                    {
                        result = null;
                        return false;
                    }
                    args.Add(argResult!);
                }

                result = generic.GenericType.Construct([.. args]);
                return true;

            case TypeReference.Tuple tuple:
                List<ITypeSymbol> elements = [];
                foreach (var element in tuple.Elements)
                {
                    if (!TryGetSymbol(element, out ITypeSymbol? elementResult))
                    {
                        result = null;
                        return false;
                    }
                    elements.Add(elementResult!);
                }
                result = Compilation.CreateTupleTypeSymbol([.. elements]);
                return true;

            case TypeReference.Array array:
                if (!TryGetSymbol(array.ElementType, out ITypeSymbol? arrayResult))
                {
                    result = null;
                    return false;
                }
                result = Compilation.CreateArrayTypeSymbol(arrayResult!);
                return true;

            default:
                result = null;
                return false;
        }
    }
}
