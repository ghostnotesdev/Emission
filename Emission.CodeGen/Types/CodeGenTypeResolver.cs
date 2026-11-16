using Emission.Builders;
using Emission.Types;
using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Types;

public class CodeGenTypeResolver : TypeResolver, ICodeGenTypeResolver
{
    public INamespaceSymbol BuilderNamespace => GetNamespace(KnownTypes.BuildersNamespace);
    public INamedTypeSymbol IBuilder => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.IBuilder));
    public INamedTypeSymbol ITypeBuilder => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.ITypeBuilder));
    public INamedTypeSymbol IStatementBuilder => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.IStatementBuilder));
    public INamedTypeSymbol ICustomStatementBuilder => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.ICustomStatementBuilder));
    public INamedTypeSymbol CompilationExpression => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.CompilationExpression));
    public INamedTypeSymbol TypeReference => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.TypeReference));
    public INamedTypeSymbol TypeResolutionResult => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.TypeResolutionResult));
    public INamedTypeSymbol ITypeSymbol => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.ITypeSymbol));
    public INamedTypeSymbol SymbolResolutionResult => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.SymbolResolutionResult));
    public INamedTypeSymbol ActionT(TypeQueryArgument t) => GetSymbol(TypeQuery.FromMetadataName(KnownTypes.ActionT, t));
    public INamedTypeSymbol String => GetSymbol(TypeQuery.FromSpecialType(SpecialType.System_String));

    public IEnumerable<INamedTypeSymbol> GetAllCustomStatementBuilders()
    {
        return BuilderNamespace
            .GetTypeMembers()
            .Where(type => type.TypeKind == TypeKind.Interface && type.AllInterfaces.Contains(ICustomStatementBuilder, SymbolEqualityComparer.Default));
    }

    private INamespaceSymbol GetNamespace(string _namespace)
    {
        NamespaceLookupResult lookup = ResolveNamespace(_namespace);
        if (!lookup.Success)
            throw lookup.Exception;

        return lookup.Result;
    }

    private INamedTypeSymbol GetSymbol(TypeQuery query)
    {
        SymbolLookupResult lookup = ResolveSymbol(query);
        if (!lookup.Success)
            throw lookup.Exception;

        return lookup.NamedSymbol;
    }
}
