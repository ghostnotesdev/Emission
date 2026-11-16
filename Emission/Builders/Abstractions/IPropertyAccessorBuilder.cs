
namespace Emission.Builders;
public interface IPropertyAccessorBuilder<TInterface> : IBuilder where TInterface : IPropertyAccessorBuilder<TInterface>
{
    IExpressionBuilder? Expression { get; }
    IBodyExpressionBuilder? Body { get; }
    bool IsAuto { get; }
    TInterface ConfigureExpressionAccessor(Action<IExpressionBuilder> configure);
    TInterface UseExpressionAccessor(IExpressionBuilder builder);
    TInterface ConfigureBodyAccessor(Action<IBodyExpressionBuilder> configure);
    TInterface UseBodyAccessor(IBodyExpressionBuilder builder);
    void Build(ICompilationStringBuilder icsb, bool includeAccessor = true);
}
