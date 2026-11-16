using Emission.Types;
using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Types;

public interface ICodeGenTypeResolver : ITypeResolver
{
    INamespaceSymbol BuilderNamespace { get; }
    INamedTypeSymbol IBuilder { get; }
    INamedTypeSymbol ITypeBuilder { get; }
    INamedTypeSymbol IStatementBuilder { get; }
    INamedTypeSymbol ICustomStatementBuilder { get; }
    INamedTypeSymbol CompilationExpression { get; }
    INamedTypeSymbol TypeReference { get; }
    INamedTypeSymbol TypeResolutionResult { get; }
    INamedTypeSymbol ITypeSymbol { get; }
    INamedTypeSymbol SymbolResolutionResult { get; }
    INamedTypeSymbol ActionT(TypeQueryArgument t);
    INamedTypeSymbol String { get; }

    IEnumerable<INamedTypeSymbol> GetAllCustomStatementBuilders();
}
