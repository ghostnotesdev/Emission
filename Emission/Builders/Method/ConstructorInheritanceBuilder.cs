using Emission.Extensions;
using Emission.Types;
using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
public class ConstructorInheritanceBuilder : BuilderBase, IConstructorInheritanceBuilder
{
    private readonly OptionalBuilderProperty<ConstructorInheritanceBuilder, ITypeBuilder, TypeBuilder> _baseClass;
    private readonly BuilderEnumerableProperty<ConstructorInheritanceBuilder, IStatementParameterBuilder, StatementParameterBuilder> _parameters;
    
    public virtual ITypeBuilder? BaseClassType => _baseClass.Builder;
    public virtual IEnumerable<IStatementParameterBuilder> Parameters => _parameters.Entries;

    public ConstructorInheritanceBuilder()
    {
        _baseClass = new(this);
        _parameters = new(this);
    }

    public IConstructorInheritanceBuilder ConfigureBaseClassType(Action<ITypeBuilder> configure) => _baseClass.Configure(configure);
    public IConstructorInheritanceBuilder UseBaseClassType(ITypeBuilder builder) => _baseClass.Use(builder);
    public IConstructorInheritanceBuilder UseParameters(IEnumerable<IStatementParameterBuilder> entries) => _parameters.Use(entries);
    public IConstructorInheritanceBuilder WithParameter(Action<IStatementParameterBuilder> configure) => _parameters.With(configure);
    public IConstructorInheritanceBuilder WithParameter(IStatementParameterBuilder builder) => _parameters.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        bool canValidateSignature = true;
        if (BaseClassType is not null)
        {
            foreach (var diag in PropagateDiagnostics(BaseClassType.Validate()))
            {
                yield return diag;
                canValidateSignature = false;
            }
        }
        else
        {
            canValidateSignature = false;
        }

        foreach (var parameter in Parameters)
        {
            foreach (var diag in PropagateDiagnostics(parameter.Validate()))
            {
                yield return diag;
                canValidateSignature = false;
            }

            canValidateSignature = canValidateSignature && parameter.ParameterType?.Reference is not null;
        }

        // If provided type reference is a symbol, validate the constructor exists. Otherwise trust the mappings got it correct.
        if (canValidateSignature && BaseClassType?.Reference is TypeReference.Symbol { Type: INamedTypeSymbol symbol })
        {
            var match = symbol.InstanceConstructors.SingleMatchOrDefault(Parameters.Select(m => m.ParameterType!.Reference!));
            if (match is null)
                yield return new MissingConstructorDiagnostic(symbol, Parameters);
        }
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        icsb.EnsureSpace()
            .Append(": base(")
            .AppendWithSeparator(", ", Parameters.Select(p => new StatementBuilder().UseStatement(p.Build)))
            .Append(")");
    }
}
