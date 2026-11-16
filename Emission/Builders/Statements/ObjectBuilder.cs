using Microsoft.CodeAnalysis;
using Emission.Extensions;
using Emission.Types;
using Emission.Validation;

namespace Emission.Builders;
public class ObjectBuilder : BuilderBase, IObjectBuilder
{
    private readonly BuilderProperty<ObjectBuilder, ITypeBuilder, TypeBuilder> _type;
    private readonly BuilderEnumerableProperty<ObjectBuilder, IStatementParameterBuilder, StatementParameterBuilder> _parameters;

    public virtual ITypeBuilder Type => _type.Builder;
    public virtual IEnumerable<IStatementParameterBuilder> Parameters => _parameters.Entries;

    public ObjectBuilder()
    {
        _type = new(this);
        _parameters = new(this);
    }

    public IObjectBuilder ConfigureType(Action<ITypeBuilder> configure) => _type.Configure(configure);
    public IObjectBuilder UseType(ITypeBuilder builder) => _type.Use(builder);
    public IObjectBuilder UseParameters(IEnumerable<IStatementParameterBuilder> builders) => _parameters.Use(builders);
    public IObjectBuilder WithParameter(Action<IStatementParameterBuilder> configure) => _parameters.With(configure);
    public IObjectBuilder WithParameter(IStatementParameterBuilder builder) => _parameters.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        bool canValidateSignature = true;
        foreach (var diag in PropagateDiagnostics(Type.Validate()))
        {
            yield return diag;
            canValidateSignature = false;
        }

        foreach (var parameter in Parameters)
        {
            foreach (var diag in PropagateDiagnostics(parameter.Validate()))
                yield return diag;

            canValidateSignature = canValidateSignature && parameter.ParameterType?.Reference is not null;
        }

        // If provided type reference is a symbol, validate the constructor exists. Otherwise trust the mappings got it correct.
        if (canValidateSignature && Type.Reference is TypeReference.Symbol { Type: INamedTypeSymbol symbol })
        {
            var match = symbol.InstanceConstructors.SingleMatchOrDefault(Parameters.Select(m => m.ParameterType!.Reference!));
            if (match is null)
                yield return new MissingConstructorDiagnostic(symbol, Parameters);
        }
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        icsb.Append("new ");
        Type.BuildType(icsb);
        icsb.Append("(")
            .AppendWithSeparator(", ", Parameters.Select(p => new StatementBuilder().UseStatement(p.Build)))
            .Append(")");
    }
}
