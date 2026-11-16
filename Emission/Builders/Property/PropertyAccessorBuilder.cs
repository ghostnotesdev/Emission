using Emission.Validation;

namespace Emission.Builders;
public abstract class PropertyAccessorBuilder<TImplementation, TInterface> : BuilderBase, IPropertyAccessorBuilder<TInterface>
    where TImplementation : PropertyAccessorBuilder<TImplementation, TInterface>, TInterface
    where TInterface : IPropertyAccessorBuilder<TInterface>
{
    private readonly BuilderExpressionProperty<TImplementation> _expression;

    public virtual IExpressionBuilder? Expression => _expression.Expression;
    public virtual IBodyExpressionBuilder? Body => _expression.Body;
    public bool IsAuto => Expression is null && Body is null;

    public PropertyAccessorBuilder()
    {
        _expression = new((TImplementation)this);
    }

    public TInterface ConfigureExpressionAccessor(Action<IExpressionBuilder> configure) => _expression.ConfigureExpression(configure);
    public virtual TInterface UseExpressionAccessor(IExpressionBuilder builder) => _expression.UseExpression(builder);
    public TInterface ConfigureBodyAccessor(Action<IBodyExpressionBuilder> configure) => _expression.ConfigureBody(configure);
    public virtual TInterface UseBodyAccessor(IBodyExpressionBuilder builder) => _expression.UseBody(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in _expression.Validate(Expression, Body, true))
            yield return diag;
    }

    public void Build(ICompilationStringBuilder icsb, bool includeAccessor = true)
    {
        if (includeAccessor)
            icsb.Append(AccessorString);
        if (Expression is not null)
            Expression.Build(icsb);
        else if (Body is not null)
            Body.Build(icsb, true);
        else
            icsb.EnsureSemicolon();
    }

    protected abstract string AccessorString { get; }
}
