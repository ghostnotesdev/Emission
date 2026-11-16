# Emission

**An extensible, validation-focused fluent API for building and emitting C# source code with Roslyn.**

Emission provides a structured builder API for generating C# source code without requiring consumers to manually assemble source strings or work directly with Roslyn syntax trees.

The library focuses specifically on **source construction**. How definitions are discovered, resolved, or transformed is left entirely to the consumer.

## Features

- Fluent builders for common C# language constructs
- Built-in structural validation
- Hierarchical diagnostics that preserve the originating builder
- Support for nested and generic types
- Configurable inheritance, modifiers, fields, properties, constructors, and methods
- Extensible builder interfaces and implementations
- Escape hatches for generation scenarios that don't fit the built-in abstractions
- No dependency on a particular source-generator architecture or application model

## Example

```csharp
// Ensures the type exists in the compilation or generates diagnostics within the builders when Validate() is called if not.
TypeResolutionResult stringType = context.TypeResolver.Resolve(TypeQuery.FromSpecialType(SpecialType.System_String));
ClassBuilder personBuilder = new();

personBuilder
    .UseNamespace("Test")
    .UseName("Person")
    .ConfigureModifiers(modifiers => modifiers
        .UseAccessModifier(AccessModifier.Public))
    .WithConstructor(constructor => constructor
        .ConfigureModifiers(cModifiers => cModifiers
            .UseAccessModifier(AccessModifier.Public))
        .WithParameter(parameter => parameter
            .UseName("name")
            .UseType(stringType))
        .ConfigureBodyExpression(body => body
            .WithStatement("this.Name = name;")))
    .WithProperty(property => property
        .ConfigureModifiers(modifiers => modifiers
            .UseAccessModifier(AccessModifier.Public))
        .UseName("Name")
        .UseType(stringType))
    .WithMethod(method => method
        .ConfigureModifiers(modifiers => modifiers
            .UseAccessModifier(AccessModifier.Public))
        .UseName("GetDisplayName")
        .UseReturnType(stringType)
        .ConfigureBodyExpression(body => body
            .WithStatement("var displayName = this.Name;")
            .WithStatement("return displayName;")
        ));

context.AddClassSource(personBuilder);
```

Alternatively, `ClassBuilder` can be extended directly and properties can be overridden to get the same result.
You can even mix and match them to pre-configure classes that have slightly different implementations.
This applies to all other builders as well, so the same approach can be used in method builders, property builders, etc.

```csharp
public class PersonBuilder(IGeneratorContext context) : ClassBuilder
{
    public override string Namespace => "Test";

    public override string Name => "Person";

    public override IClassModifierBuilder ModifierBuilder => new ClassModifierBuilder()
        .UseAccessModifier(AccessModifier.Public);

    public override IEnumerable<IConstructorBuilder> Constructors => [
        new ConstructorBuilder()
            .ConfigureModifiers(cModifiers => cModifiers
                .UseAccessModifier(AccessModifier.Public))
            .WithParameter(parameter => parameter
                .UseName("name")
                .UseType(context.TypeResolver.Resolve(TypeQuery.FromSpecialType(SpecialType.System_String))))
            .ConfigureBodyExpression(body => body
                .WithStatement("this.Name = name;"))
    ];

    public override IEnumerable<IPropertyBuilder> Properties => [
        new PropertyBuilder()
            .ConfigureModifiers(modifiers => modifiers
                .UseAccessModifier(AccessModifier.Public))
            .UseName("Name")
            .UseType(context.TypeResolver.Resolve(TypeQuery.FromSpecialType(SpecialType.System_String)))
    ];

    public override IEnumerable<IMethodBuilder> Methods => [
        new MethodBuilder()
           .ConfigureModifiers(modifiers => modifiers
                .UseAccessModifier(AccessModifier.Public))
            .UseName("GetDisplayName")
            .UseReturnType(context.TypeResolver.Resolve(TypeQuery.FromSpecialType(SpecialType.System_String)))
            .ConfigureBodyExpression(body => body
                .WithStatement("var displayName = this.Name;")
                .WithStatement("return displayName;")
            )
    ];
}
```

The examples above will resolve to this:

```csharp
namespace Test;

public class Person
{
    public Person(string name)
    {
        this.Name = name;
    }

    public string Name { get; set; }

    public string GetDisplayName()
    {
        var displayName = this.Name;
        return displayName;
    }
}
```

Type-consuming builders accept either a custom `ITypeBuilder` or a direct
`TypeReference`/`TypeResolutionResult`. Specialized statements can also be
configured directly as statement parameters:

```csharp
var parameter = new ObjectBuilder()
    .UseType(new TypeReference.Generated("Person"))
    .WithParameter(parameter => parameter
        .UseType(stringType)
        .UseStatement(@"""John Smith"""));

// or:
var parameter = new ObjectBuilder()
    .UseType(personBuilder)
    .WithParameter(parameter => parameter
        .UseType(stringType)
        .UseStatement(@"""John Smith"""));
```

The `ObjectBuilder` example above will resolve this this:

```csharp
new global::Test.Person("John Smith")
```

Emission represents the structure of the generated source and can validate that structure before emitting it.

```csharp
IEnumerable<IDiagnostic> diagnostics = builder.Validate();

if (!diagnostics.Any())
{
    CompilationStringBuilder source = new();
    builder.Build(source);

    Console.WriteLine(source);
}
```

## Philosophy

Emission deliberately does not own the process that determines **what** source code should be generated.

A consumer might derive its definitions from:

- Roslyn symbols
- XML or JSON
- schemas
- reflection
- configuration
- another compiler or generator
- application-specific metadata

Those concerns belong to the consuming application.

Emission begins once the consumer knows what C# structure it wants to create:

```text
Consumer-defined information
          |
          v
     Emission builders
          |
          v
       Validation
          |
          v
      C# source code
```

This keeps the library independent of any particular source-generation architecture while allowing consumers to build higher-level abstractions on top of it.

## Extensibility

The built-in builders are intended to make common source-generation scenarios quick to express without preventing more specialized use cases.

Builders can be supplied directly:

```csharp
classBuilder.WithMethod(new CustomMethodBuilder());
```

This allows applications and libraries to create reusable domain-specific builders while continuing to compose them with Emission's standard builders.

For one-off generation scenarios, lower-level source construction can be used where representing the behavior through additional fluent abstractions would add unnecessary complexity.

The goal is to provide useful abstractions without requiring every possible C# construct or generation pattern to become part of Emission itself.

## Validation

Builders expose validation independently from source emission:

```csharp
IEnumerable<IDiagnostic> diagnostics = builder.Validate();
```

Diagnostics propagate through the builder hierarchy, allowing errors originating in nested builders to retain information about the structure in which they occurred.

This makes it possible for source generators to report useful diagnostics without coupling Emission to the consumer's generator context or diagnostic policy.

## Source Generators

Emission is designed to work well with Roslyn incremental source generators while remaining independent of them.

Your generator remains responsible for:

- obtaining inputs
- resolving Roslyn symbols
- transforming domain-specific information
- configuring Emission builders
- deciding how diagnostics are reported
- adding generated source to the compilation

Emission is responsible for:

- representing C# source structures
- validating builder configuration
- emitting those structures as C# source

This separation allows the same builder infrastructure to be reused across generators with very different input models and architectures.

## Status

Emission is under active development.

The initial API focuses on the C# constructs required by its current consumers. Additional functionality will be added as real-world generation scenarios require it rather than attempting to model the entire C# language upfront.

## License

See [LICENSE](LICENSE) for license information.
