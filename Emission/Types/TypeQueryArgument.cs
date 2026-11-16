using Microsoft.CodeAnalysis;

namespace Emission.Types;

public abstract class TypeQueryArgument
{
    private TypeQueryArgument() { }

    public sealed class Query(TypeQuery value) : TypeQueryArgument
    {
        public TypeQuery Value { get; } = value;
    }

    public sealed class Reference(TypeReference? value) : TypeQueryArgument
    {
        public TypeReference? Value { get; } = value;
    }

    public sealed class Resolution(TypeResolutionResult value) : TypeQueryArgument
    {
        public TypeResolutionResult Value { get; } = value;
    }

    public static implicit operator TypeQueryArgument(TypeQuery query)
    {
        return new Query(query);
    }

    public static implicit operator TypeQueryArgument(TypeReference? reference)
    {
        return new Reference(reference);
    }

    public static implicit operator TypeQueryArgument(TypeResolutionResult resolution)
    {
        return new Resolution(resolution);
    }
}
