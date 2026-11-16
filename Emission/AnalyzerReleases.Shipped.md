; Shipped analyzer releases
; https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0

### New Rules

Rule ID | Category      | Severity | Notes
--------|---------------|----------|--------------------
E001    | CodeGen       |  Error   | E001_UnhandledError, [Documentation](E001_Documentation_Link)
E002    | Validation    |  Error   | E002_BuilderValidationError, [Documentation](E002_Documentation_Link)
E003    | Validation    |  Error   | E003_CompilationDesyncError, [Documentation](E003_Documentation_Link)
E004    | Compilation   |  Error   | E004_MissingTypeError, [Documentation](E004_Documentation_Link)
E005    | Compilation   |  Error   | E005_MissingConstructorError, [Documentation](E005_Documentation_Link)
E006    | Compilation   |  Error   | E006_InvalidTypeError, [Documentation](E006_Documentation_Link)
E007    | Compilation   |  Error   | E007_MissingMethodError, [Documentation](E007_Documentation_Link)
E008    | Compilation   |  Error   | E008_AmbiguousTypeError, [Documentation](E008_Documentation_Link)
E009    | Compilation   |  Error   | E009_SymbolLookupError, [Documentation](E009_Documentation_Link)
E010    | Compilation   |  Error   | E010_NamespaceLookupError, [Documentation](E010_Documentation_Link)