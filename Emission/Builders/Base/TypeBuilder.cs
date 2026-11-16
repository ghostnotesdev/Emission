using Emission.Extensions;
using Emission.Types;
using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
// NOTE:
// Declaration builders implement ITypeBuilder so they can be used directly
// anywhere their declared type is referenced. Their type identity is inferred
// from the declaration and therefore cannot be changed with UseType(...).
//
// This is intentional. Although separating reference ownership into an
// ITypeReferenceProvider interface would make the interface hierarchy stricter,
// doing so would introduce another abstraction throughout the builder API.
//
// Reconsider that split only if the unsupported ITypeBuilder operations cause
// practical ambiguity or misuse significant enough to justify the additional
// abstraction.
public class TypeBuilder : BuilderBase, ITypeBuilder
{
    private IEnumerable<IDiagnostic>? _typeLookupErrors;
    public TypeReference? Reference { get; private set; }
    public override string DiagnosticName => Reference?.ToDisplayString() ?? "<unnamed type>";

    public ITypeBuilder UseType(TypeReference typeReference)
    {
        Reference = typeReference;
        return this;
    }

    public ITypeBuilder UseType(TypeResolutionResult resolutionResult)
    {
        Reference = resolutionResult.Result;
        _typeLookupErrors = resolutionResult.Diagnostics;
        return this;
    }

    public ITypeBuilder UseType(ITypeSymbol symbol)
    {
        Reference = symbol.TypeReference();
        return this;
    }

    public ITypeBuilder UseType(SymbolLookupResult symbolResult)
    {
        Reference = symbolResult.Result?.TypeReference();
        _typeLookupErrors = symbolResult.Diagnostics;
        return this;
    }

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (_typeLookupErrors?.Any() == true)
        {
            foreach (var diag in PropagateDiagnostics(_typeLookupErrors))
                yield return diag;
        }
        else if (Reference is null)
        {
            yield return new InvalidBuilderConfigurationDiagnostic(this, $".{nameof(UseType)}() was never called.");
        }
    }

    public void BuildType(ICompilationStringBuilder icsb)
    {
        if (Reference is null)
            throw new InvalidOperationException("Build was called on type builder without a valid reference");

        icsb.Append(Reference.Tokenize());
    }
}
