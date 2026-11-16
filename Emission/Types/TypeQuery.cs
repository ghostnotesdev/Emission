using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Emission.Types;

public class TypeQuery
{
    private readonly List<TypeQueryArgument> _typeArguments;

    public enum QueryMode
    {
        NameSearch,
        MetadataReference,
        SpecialType
    }

    public string Name { get; private set; }
    public string MetadataName { get; private set; }
    public SpecialType SpecialType { get; private set; }
    public bool IsArray { get; private set; }
    public int ArrayRank { get; private set; }
    public TypeQueryArgument[] TypeArguments => [.. _typeArguments];
    public QueryMode Mode { get; private set; }

    private TypeQuery(string name, string metadataName, SpecialType specialType, TypeQueryArgument[] typeArguments, QueryMode mode)
    {
        Name = name;
        MetadataName = metadataName;
        SpecialType = specialType;
        _typeArguments = [.. typeArguments];
        Mode = mode;
    }

    public TypeQuery WithTypeArgumentFromName(string name, params TypeQueryArgument[] typeArguments)
        => WithTypeArgumentFromName(name, (IEnumerable<TypeQueryArgument>)typeArguments);

    public TypeQuery WithTypeArgumentFromName(string name, IEnumerable<TypeQueryArgument> typeArguments)
    {
        _typeArguments.Add(FromName(name, typeArguments));
        return this;
    }

    public TypeQuery WithTypeArgumentFromMetadataName(string metadataName, params TypeQueryArgument[] typeArguments)
        => WithTypeArgumentFromMetadataName(metadataName, (IEnumerable<TypeQueryArgument>)typeArguments);

    public TypeQuery WithTypeArgumentFromMetadataName(string metadataName, IEnumerable<TypeQueryArgument> typeArguments)
    {
        _typeArguments.Add(FromMetadataName(metadataName, typeArguments));
        return this;
    }

    public TypeQuery WithTypeArgumentFromSpecialType(SpecialType specialType, params TypeQueryArgument[] typeArguments)
        => WithTypeArgumentFromSpecialType(specialType, (IEnumerable<TypeQueryArgument>)typeArguments);

    public TypeQuery WithTypeArgumentFromSpecialType(SpecialType specialType, IEnumerable<TypeQueryArgument> typeArguments)
    {
        _typeArguments.Add(FromSpecialType(specialType, typeArguments));
        return this;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        switch (Mode)
        {
            case QueryMode.NameSearch:
                sb.Append(Name);
                break;
            case QueryMode.MetadataReference:
                sb.Append(MetadataName);
                break;
            case QueryMode.SpecialType:
                sb.Append(SpecialType.ToString());
                break;
        }

        return sb
            .Append(TypeArguments.Length > 0 ? "`" + TypeArguments.Length : string.Empty)
            .Append(IsArray ? "[]" : string.Empty)
            .ToString();
    }

    public TypeQuery AsArray(int rank = 1)
    {
        IsArray = true;
        ArrayRank = rank;
        return this;
    }

    public static TypeQuery FromName(string name, params TypeQueryArgument[] typeArguments) =>
        FromName(name, (IEnumerable<TypeQueryArgument>)typeArguments);

    public static TypeQuery FromName(string name, IEnumerable<TypeQueryArgument> typeArguments) =>
        new(name, string.Empty, SpecialType.None, [.. typeArguments], QueryMode.NameSearch);

    public static TypeQuery FromMetadataName(string metadataName, params TypeQueryArgument[] typeArguments) =>
        FromMetadataName(metadataName, (IEnumerable<TypeQueryArgument>)typeArguments);

    public static TypeQuery FromMetadataName(string metadataName, IEnumerable<TypeQueryArgument> typeArguments) =>
        new(string.Empty, metadataName, SpecialType.None, [.. typeArguments], QueryMode.MetadataReference);

    public static TypeQuery FromSpecialType(SpecialType specialType, params TypeQueryArgument[] typeArguments) =>
        FromSpecialType(specialType, (IEnumerable<TypeQueryArgument>)typeArguments);

    public static TypeQuery FromSpecialType(SpecialType specialType, IEnumerable<TypeQueryArgument> typeArguments) =>
        new(string.Empty, string.Empty, specialType, [.. typeArguments], QueryMode.SpecialType);
}
