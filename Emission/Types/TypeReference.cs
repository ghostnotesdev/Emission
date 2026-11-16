using Emission.Builders;
using Emission.Extensions;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Emission.Types;

public abstract class TypeReference
{
    public sealed class Symbol(ITypeSymbol type) : TypeReference
    {
        public ITypeSymbol Type => type;
    }

    public sealed class Generated(string token) : TypeReference
    {
        public string Token => token;
    }

    public sealed class Generic(INamedTypeSymbol genericType, params TypeReference[] typeParameters) : TypeReference
    {
        public Generic(INamedTypeSymbol genericType, IEnumerable<TypeReference> typeParameters)
            : this(genericType, [.. typeParameters])
        {
        }

        public INamedTypeSymbol GenericType => genericType;
        public TypeReference[] TypeParameters => typeParameters;
    }

    public sealed class Tuple(params TypeReference[] elements) : TypeReference
    {
        public Tuple(IEnumerable<TypeReference> elements)
            : this([.. elements])
        {
        }

        public TypeReference[] Elements => elements;
    }

    public sealed class Array(TypeReference elementType, int rank = 1) : TypeReference
    {
        public TypeReference ElementType => elementType;
        public int Rank => rank;
    }

    public string Tokenize()
    {
        StringBuilder sb = new();
        switch (this)
        {
            case Symbol symbol:
                sb.Append(symbol.Type.Tokenize());
                break;

            case Generated generated:
                sb.Append(generated.Token);
                break;

            case Generic generic:
                sb.Append(generic.GenericType.TokenizeGenericType());
                if (generic.TypeParameters.Length > 0)
                {
                    sb.Append("<")
                      .Append(string.Join(", ", generic.TypeParameters.Select(p => p.Tokenize())))
                      .Append(">");
                }
                break;

            case Tuple tuple:
                sb.Append("(");
                sb.Append(string.Join(", ", tuple.Elements.Select(e => e.Tokenize())));
                sb.Append(")");
                break;
            case Array array:
                sb.Append(array.ElementType.Tokenize());
                sb.Append("[");
                for (int i = 2; i < array.Rank; i++)
                    sb.Append(",");
                sb.Append("]");
                break;
        }

        return sb.ToString();
    }

    public override string ToString()
    {
        return Tokenize();
    }

    public Array AsArray(int rank = 1) => new(this, rank);

    public string ToDisplayString() => Tokenize().Replace("global::", "");

    public ITypeBuilder ToTypeBuilder() => new TypeBuilder().UseType(this);
}
