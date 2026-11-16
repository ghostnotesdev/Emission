namespace Emission.CodeGen.Types;

public static class KnownTypes
{
    #region Emission.Builders
    public const string BuildersNamespace = "Emission.Builders";
    private const string _emission_builders = BuildersNamespace + ".";
    public const string IBuilder = _emission_builders + nameof(IBuilder);
    public const string ITypeBuilder = _emission_builders + nameof(ITypeBuilder);
    public const string IStatementBuilder = _emission_builders + nameof(IStatementBuilder);
    public const string ICustomStatementBuilder = _emission_builders + nameof(ICustomStatementBuilder);
    public const string CompilationExpression = _emission_builders + nameof(CompilationExpression);
    public static string GetMetadataNameForBuilder(string builder) => _emission_builders + builder;

    #endregion

    #region Emission.Types

    private const string _emission_types = "Emission.Types.";
    public const string TypeReference = _emission_types + nameof(TypeReference);
    public const string TypeResolutionResult = _emission_types + nameof(TypeResolutionResult);
    public const string SymbolResolutionResult = _emission_types + nameof(SymbolResolutionResult);

    #endregion

    #region Microsoft.CodeAnalysis

    private const string _microsoft_code_analysis = "Microsoft.CodeAnalysis.";
    public const string ITypeSymbol = _microsoft_code_analysis + nameof(ITypeSymbol);

    #endregion

    #region System

    private const string _system = "System.";
    public const string Action = _system + nameof(Action);
    public const string ActionT = _system + nameof(Action) + "`1";

    #endregion
}
