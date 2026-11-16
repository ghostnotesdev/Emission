using Emission.Validation;

namespace Emission.Builders;
public class ConstructorBuilder : BuilderBase, IConstructorBuilder
{
    private readonly BuilderProperty<ConstructorBuilder, IConstructorModifierBuilder, ConstructorModifierBuilder> _modifier;
    private readonly BuilderExpressionProperty<ConstructorBuilder> _expression;
    private readonly BuilderEnumerableProperty<ConstructorBuilder, IMethodParameterBuilder, MethodParameterBuilder> _parameters;
    private readonly OptionalBuilderProperty<ConstructorBuilder, IConstructorInheritanceBuilder, ConstructorInheritanceBuilder> _baseConstructor;

    public virtual IConstructorModifierBuilder Modifier => _modifier.Builder;
    public virtual IExpressionBuilder? Expression => _expression.Expression;
    public virtual IBodyExpressionBuilder? Body => _expression.Body;
    public virtual IEnumerable<IMethodParameterBuilder> Parameters => _parameters.Entries;
    public virtual IConstructorInheritanceBuilder? BaseConstructor => _baseConstructor.Builder;

    public ConstructorBuilder()
    {
        _modifier = new(this);
        _expression = new(this);
        _parameters = new(this);
        _baseConstructor = new(this);
    }

    public IConstructorBuilder ConfigureModifiers(Action<IConstructorModifierBuilder> configure) => _modifier.Configure(configure);
    public IConstructorBuilder UseModifiers(IConstructorModifierBuilder builder) => _modifier.Use(builder);
    public IConstructorBuilder ConfigureExpression(Action<IExpressionBuilder> configure) => _expression.ConfigureExpression(configure);
    public IConstructorBuilder UseExpression(IExpressionBuilder builder) => _expression.UseExpression(builder);
    public IConstructorBuilder ConfigureBodyExpression(Action<IBodyExpressionBuilder> configure) => _expression.ConfigureBody(configure);
    public IConstructorBuilder UseBodyExpression(IBodyExpressionBuilder builder) => _expression.UseBody(builder);
    public IConstructorBuilder UseParameters(IEnumerable<IMethodParameterBuilder> builders) => _parameters.Use(builders);
    public IConstructorBuilder WithParameter(Action<IMethodParameterBuilder> configure) => _parameters.With(configure);
    public IConstructorBuilder WithParameter(IMethodParameterBuilder builder) => _parameters.With(builder);
    public IConstructorBuilder ConfigureBaseConstructor(Action<IConstructorInheritanceBuilder> configure) => _baseConstructor.Configure(configure);
    public IConstructorBuilder UseBaseConstructor(IConstructorInheritanceBuilder builder) => _baseConstructor.Use(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(Modifier.Validate()))
            yield return diag;

        foreach (var diag in _expression.Validate(Expression, Body, true))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Parameters.SelectMany(parameter => parameter.Validate())))
            yield return diag;

        if (BaseConstructor is not null)
            foreach (var diag in PropagateDiagnostics(BaseConstructor.Validate()))
                yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb, string className)
    {
        Modifier.Build(icsb);
        icsb.EnsureSpace()
            .Append($"{className}(")
            .AppendWithSeparator(", ", Parameters.Select(parameter => new StatementBuilder().UseStatement(parameter.Build)))
            .Append(")");

        BaseConstructor?.Build(icsb);

        if (Expression is not null)
        {
            Expression.Build(icsb);
        }
        else if (Body is not null)
        {
            Body.Build(icsb, true);
        }
        else
        {
            icsb.EnsureSpace()
                .AppendLine("{ }");
        }
    }
}
