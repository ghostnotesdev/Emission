using Emission.Types;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
public interface ITypeBuilder : IBuilder
{
    TypeReference? Reference { get; }
    ITypeBuilder UseType(TypeReference typeReference);
    ITypeBuilder UseType(TypeResolutionResult resolutionResult);
    ITypeBuilder UseType(ITypeSymbol symbol);
    ITypeBuilder UseType(SymbolLookupResult symbolResult);
    void BuildType(ICompilationStringBuilder icsb);
}
