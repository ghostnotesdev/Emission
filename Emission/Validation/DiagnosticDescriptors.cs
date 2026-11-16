using Microsoft.CodeAnalysis;

namespace Emission.Validation;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor UnhandledError = new(
        id: "E001",
        title: "Unexpected Error",
        messageFormat: "An error occurred in {0}: {1}",
        category: "CodeGen",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor BuilderValidationError = new(
        id: "E002",
        title: "Builder Validation Error",
        messageFormat: "{0}",
        category: "Validation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidBuilderConfigurationError = new(
        id: "E003",
        title: "Invalid Builder Configuration Error",
        messageFormat: "{0}",
        category: "Validation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingTypeError = new(
        id: "E004",
        title: "Missing Type Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingConstructorError = new(
        id: "E005",
        title: "Missing Constructor Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidTypeError = new(
        id: "E006",
        title: "Invalid Type Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingMethodError = new(
        id: "E007",
        title: "Missing Method Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AmbiguousTypeError = new(
        id: "E008",
        title: "Ambiguous Type Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SymbolLookupError = new(
        id: "E009",
        title: "Symbol Lookup Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NamespaceLookupError = new(
        id: "E010",
        title: "Namespace Lookup Error",
        messageFormat: "{0}",
        category: "Compilation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
