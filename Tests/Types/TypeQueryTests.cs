using Emission.Types;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Emission.UnitTests.Types;

public class TypeQueryTests
{
    [Fact]
    public void FromName_BuildsNameSearchQuery()
    {
        var query = TypeQuery.FromName("List").WithTypeArgumentFromSpecialType(SpecialType.System_String);

        Assert.Equal(TypeQuery.QueryMode.NameSearch, query.Mode);
        Assert.Equal("List`1", query.ToString());
        Assert.Equal("List", query.Name);
    }

    [Fact]
    public void FromMetadataName_BuildsMetadataQuery()
    {
        var query = TypeQuery.FromMetadataName("System.String");

        Assert.Equal(TypeQuery.QueryMode.MetadataReference, query.Mode);
        Assert.Equal("System.String", query.ToString());
        Assert.Equal("System.String", query.MetadataName);
    }

    [Fact]
    public void FromSpecialType_BuildsSpecialTypeQuery()
    {
        var query = TypeQuery.FromSpecialType(SpecialType.System_Int32);

        Assert.Equal("System_Int32", query.ToString());
        Assert.Equal(SpecialType.System_Int32, query.SpecialType);
    }

    [Fact]
    public void FromName_AcceptsEnumerableTypeArguments()
    {
        IEnumerable<TypeQueryArgument> arguments = [TypeQuery.FromName("T")];

        var query = TypeQuery.FromName("List", arguments);

        Assert.Single(query.TypeArguments);
    }

    [Fact]
    public void WithTypeArgumentFromName_AcceptsEnumerableTypeArguments()
    {
        IEnumerable<TypeQueryArgument> arguments = [TypeQuery.FromName("T")];

        var query = TypeQuery.FromName("Dictionary")
            .WithTypeArgumentFromName("List", arguments);

        Assert.Single(query.TypeArguments);
    }

    [Fact]
    public void FromMetadataName_AcceptsEnumerableTypeArguments()
    {
        IEnumerable<TypeQueryArgument> arguments = [TypeQuery.FromName("T")];

        var query = TypeQuery.FromMetadataName("System.Collections.Generic.List", arguments);

        Assert.Single(query.TypeArguments);
    }

    [Fact]
    public void FromSpecialType_AcceptsEnumerableTypeArguments()
    {
        IEnumerable<TypeQueryArgument> arguments = [TypeQuery.FromName("T")];

        var query = TypeQuery.FromSpecialType(SpecialType.System_Object, arguments);

        Assert.Single(query.TypeArguments);
    }

    [Fact]
    public void WithTypeArgumentFromMetadataName_AcceptsEnumerableTypeArguments()
    {
        IEnumerable<TypeQueryArgument> arguments = [TypeQuery.FromName("T")];

        var query = TypeQuery.FromName("Dictionary")
            .WithTypeArgumentFromMetadataName("List", arguments);

        Assert.Single(query.TypeArguments);
    }

    [Fact]
    public void WithTypeArgumentFromSpecialType_AcceptsEnumerableTypeArguments()
    {
        IEnumerable<TypeQueryArgument> arguments = [TypeQuery.FromName("T")];

        var query = TypeQuery.FromName("Dictionary")
            .WithTypeArgumentFromSpecialType(SpecialType.System_String, arguments);

        Assert.Single(query.TypeArguments);
    }
}
