using Emission.Extensions;
using Emission.Types;
using Emission.Validation;
using Microsoft.CodeAnalysis;

namespace Emission.Builders;
public class StaticMethodCallBuilder : BuilderBase, IStaticMethodCallBuilder
{
    private readonly BuilderProperty<StaticMethodCallBuilder, IGenericArgumentsBuilder, GenericArgumentsBuilder> _generics;
    private readonly BuilderProperty<StaticMethodCallBuilder, ITypeBuilder, TypeBuilder> _class;
    private readonly BuilderIdentifierProperty<StaticMethodCallBuilder> _method;
    private readonly BuilderEnumerableProperty<StaticMethodCallBuilder, IStatementParameterBuilder, StatementParameterBuilder> _parameters;

    public virtual IGenericArgumentsBuilder Generics => _generics.Builder;
    public virtual ITypeBuilder Class => _class.Builder;
    public virtual string Method => _method.Value;
    public virtual IEnumerable<IStatementParameterBuilder> Parameters => _parameters.Entries;

    public StaticMethodCallBuilder()
    {
        _generics = new(this);
        _class = new(this);
        _method = new(this);
        _parameters = new(this);
    }

    public IStaticMethodCallBuilder CallMethod(string methodName) => _method.Use(methodName);
    public IStaticMethodCallBuilder OnClass(Action<ITypeBuilder> configure) => _class.Configure(configure);
    public IStaticMethodCallBuilder OnClass(ITypeBuilder builder) => _class.Use(builder);
    public IStaticMethodCallBuilder UseParameters(IEnumerable<IStatementParameterBuilder> builders) => _parameters.Use(builders);
    public IStaticMethodCallBuilder WithParameter(Action<IStatementParameterBuilder> configure) => _parameters.With(configure);
    public IStaticMethodCallBuilder WithParameter(IStatementParameterBuilder builder) => _parameters.With(builder);
    public IStaticMethodCallBuilder ConfigureGenericArguments(Action<IGenericArgumentsBuilder> configure) => _generics.Configure(configure);
    public IStaticMethodCallBuilder UseGenericArguments(IGenericArgumentsBuilder builder) => _generics.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        bool canValidateSignature = true;
        foreach (var diag in PropagateDiagnostics(Class.Validate()))
        {
            yield return diag;
            canValidateSignature = false;
        }

        foreach (var diag in _method.Validate(Method, nameof(Method)))
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

        if (canValidateSignature && Class.Reference is TypeReference.Symbol { Type: INamedTypeSymbol symbol })
        {
            int argCount = Generics.Arguments.Count();
            var match = symbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.IsStatic && method.TypeParameters.Length == argCount)
                .SingleMatchOrDefault(Method, Parameters.Select(mapping => mapping.ParameterType!.Reference!));

            if (match is null)
                yield return new MissingMethodDiagnostic(symbol, Parameters);
        }
    }

    public void Build(ICompilationStringBuilder icsb)
    {
        Class.BuildType(icsb);
        icsb.Append(".")
            .Append(Method!);

        Generics.BuildDeclaration(icsb);

        icsb.Append("(")
            .AppendWithSeparator(", ", Parameters.Select(p => new StatementBuilder().UseStatement(p.Build)))
            .Append(")");
    }
}
