using Emission.Types;

namespace Emission.Builders;
public static class ClassInheritanceBuilderExtensions
{
    public static IClassInheritanceBuilder UseBaseClass(this IClassInheritanceBuilder builder, TypeReference type)
        => builder.UseBaseClass(new TypeBuilder().UseType(type));

    public static IClassInheritanceBuilder UseBaseClass(this IClassInheritanceBuilder builder, TypeResolutionResult result)
        => builder.UseBaseClass(new TypeBuilder().UseType(result));

    public static IClassInheritanceBuilder Extends(this IClassInheritanceBuilder builder, TypeReference type)
        => builder.WithImplementation(new TypeBuilder().UseType(type));

    public static IClassInheritanceBuilder Extends(this IClassInheritanceBuilder builder, TypeResolutionResult result)
        => builder.WithImplementation(new TypeBuilder().UseType(result));

    public static IClassInheritanceBuilder Implements(this IClassInheritanceBuilder builder, TypeReference type)
        => builder.WithImplementation(new TypeBuilder().UseType(type));

    public static IClassInheritanceBuilder Implements(this IClassInheritanceBuilder builder, TypeResolutionResult result)
        => builder.WithImplementation(new TypeBuilder().UseType(result));
}
