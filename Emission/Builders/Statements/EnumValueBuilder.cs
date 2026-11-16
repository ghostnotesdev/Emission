using Emission.Types;
using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
public class EnumValueBuilder : BuilderBase, IEnumValueBuilder
{
    private readonly BuilderProperty<EnumValueBuilder, ITypeBuilder, TypeBuilder> _enumType;
    private readonly BuilderIdentifierProperty<EnumValueBuilder> _requestedValue;

    public virtual ITypeBuilder EnumType => _enumType.Builder;
    public virtual string RequestedValue => _requestedValue.Value;

    public EnumValueBuilder()
    {
        _enumType = new(this);
        _requestedValue = new(this);
    }

    public IEnumValueBuilder ConfigureType(Action<ITypeBuilder> configure) => _enumType.Configure(configure);
    public IEnumValueBuilder UseType(ITypeBuilder builder) => _enumType.Use(builder);
    public IEnumValueBuilder UseValue(string value) => _requestedValue.Use(value);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(EnumType.Validate()))
            yield return diag;

        foreach (var diag in _requestedValue.Validate(RequestedValue, nameof(RequestedValue)))
            yield return diag;

        foreach (var diag in ValidateEnumValue(RequestedValue))
            yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        EnumType.BuildType(icsb);
        icsb.Append(".")
            .Append(RequestedValue);
    }

    private IEnumerable<IDiagnostic> ValidateEnumValue(string requestedValue)
    {
        switch (EnumType.Reference)
        {
            case TypeReference.Symbol symbol:
                foreach (var diag in ValidateEnumSymbolValue(symbol.Type, requestedValue))
                    yield return diag;
                break;
            case TypeReference.Generated:
                break;
            case TypeReference.Generic generic:
                yield return new InvalidBuilderConfigurationDiagnostic(this, $"Generic type reference '{generic.Tokenize()}' cannot represent an enum.");
                break;
            case TypeReference.Tuple tuple:
                yield return new InvalidBuilderConfigurationDiagnostic(this, $"Tuple type reference '{tuple.Tokenize()}' cannot represent an enum.");
                break;
            default:
                yield return new InvalidBuilderConfigurationDiagnostic(this, $"'{EnumType.DiagnosticName}' is not a recognized type reference: " + EnumType.Reference?.Tokenize());
                break;
        }
    }

    private IEnumerable<IDiagnostic> ValidateEnumSymbolValue(ITypeSymbol symbol, string requestedValue)
    {
        if (symbol.TypeKind != TypeKind.Enum)
        {
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"Symbol '{symbol.ToDisplayString()}' does not represent an enum.");
        }
        else
        {
            IFieldSymbol? enumValue = symbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(field => field.IsConst && field.Name == requestedValue);

            if (enumValue is null)
            {
                yield return new InvalidBuilderConfigurationDiagnostic(this, $"Invalid enum value for {symbol.ToDisplayString()}: {requestedValue}");
            }
        }
    }
}
