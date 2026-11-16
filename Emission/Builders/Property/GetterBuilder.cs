namespace Emission.Builders;
public class GetterBuilder : PropertyAccessorBuilder<GetterBuilder, IGetterBuilder>, IGetterBuilder
{
    protected override string AccessorString => "get";
}
