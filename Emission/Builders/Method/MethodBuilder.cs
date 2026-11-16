using Emission.Extensions;
using Emission.Validation;
using System.Diagnostics.CodeAnalysis;

namespace Emission.Builders;
/// <remarks>
/// Virtual properties expose the effective state used during validation and
/// generation. Fluent configuration methods modify state owned by the base
/// implementation.
///
/// Derived builders may override individual properties to provide specialized
/// behavior. An overridden property should not also be configured through its
/// corresponding fluent methods unless the derived implementation explicitly
/// supports doing so.
/// </remarks>
public class MethodBuilder : BuilderBase, IMethodBuilder
{
    private readonly BuilderIdentifierProperty<MethodBuilder> _name;
    private readonly BuilderSwitchProperty<MethodBuilder> _isInterfaceMethod;
    private readonly BuilderIdentifierProperty<MethodBuilder> _extensionMethodParameterName;
    private readonly BuilderProperty<MethodBuilder, IMethodModifierBuilder, MethodModifierBuilder> _modifier;
    private readonly BuilderProperty<MethodBuilder, IGenericArgumentsBuilder, GenericArgumentsBuilder> _generics;
    private readonly OptionalBuilderProperty<MethodBuilder, ITypeBuilder, TypeBuilder> _returnType;
    private readonly BuilderExpressionProperty<MethodBuilder> _expression;
    private readonly BuilderEnumerableProperty<MethodBuilder, IMethodParameterBuilder, MethodParameterBuilder> _parameters;

    public virtual string MethodName => _name.Value;
    public virtual bool IsInterfaceMethod => _isInterfaceMethod.Value;
    public virtual string ExtensionMethodParameterName => _extensionMethodParameterName.Value;
    public virtual IMethodModifierBuilder Modifier => _modifier.Builder;
    public virtual IGenericArgumentsBuilder Generics => _generics.Builder;
    public virtual ITypeBuilder? ReturnType => _returnType.Builder;
    public virtual IExpressionBuilder? Expression => _expression.Expression;
    public virtual IBodyExpressionBuilder? Body => _expression.Body;
    public virtual IEnumerable<IMethodParameterBuilder> Parameters => _parameters.Entries;

    public override string DiagnosticName => !string.IsNullOrWhiteSpace(MethodName) ? MethodName : "<unnamed method>";

    public bool IsExtensionMethod => ExtensionMethodParameterName != string.Empty;

    [MemberNotNullWhen(true, nameof(ReturnType))]
    public bool IsValidExtensionMethod =>
        IsExtensionMethod
        && ExtensionMethodParameterName.IsValidIdentifier()
        && ReturnType is not null
        && Modifier.AccessModifier == AccessModifier.Public
        && Modifier.MethodModifier == MethodModifier.Static;

    public MethodBuilder()
    {
        _name = new(this);
        _isInterfaceMethod = new(this);
        _extensionMethodParameterName = new(this);
        _modifier = new(this);
        _generics = new(this);
        _returnType = new(this);
        _expression = new(this);
        _parameters = new(this);
    }

    public IMethodBuilder UseName(string methodName) => _name.Use(methodName);
    public IMethodBuilder ConfigureReturnType(Action<ITypeBuilder> configure) => _returnType.Configure(configure);
    public IMethodBuilder UseReturnType(ITypeBuilder builder) => _returnType.Use(builder);
    public IMethodBuilder ConfigureModifiers(Action<IMethodModifierBuilder> configure) => _modifier.Configure(configure);
    public IMethodBuilder UseModifiers(IMethodModifierBuilder builder) => _modifier.Use(builder);
    public IMethodBuilder ConfigureExpression(Action<IExpressionBuilder> configure) => _expression.ConfigureExpression(configure);
    public IMethodBuilder UseExpression(IExpressionBuilder builder) => _expression.UseExpression(builder);
    public IMethodBuilder ConfigureBodyExpression(Action<IBodyExpressionBuilder> configure) => _expression.ConfigureBody(configure);
    public IMethodBuilder UseBodyExpression(IBodyExpressionBuilder builder) => _expression.UseBody(builder);
    public IMethodBuilder UseParameters(IEnumerable<IMethodParameterBuilder> parameters) => _parameters.Use(parameters);
    public IMethodBuilder WithParameter(Action<IMethodParameterBuilder> configure) => _parameters.With(configure);
    public IMethodBuilder WithParameter(IMethodParameterBuilder builder) => _parameters.With(builder);
    public IMethodBuilder ConfigureGenericArguments(Action<IGenericArgumentsBuilder> configure) => _generics.Configure(configure);
    public IMethodBuilder UseGenericArguments(IGenericArgumentsBuilder builder) => _generics.Use(builder);
    public IMethodBuilder UseExtensionMethod(string parameterName) =>
        _extensionMethodParameterName
            .Use(parameterName)
            .ConfigureModifiers(mod => mod
                .UseAccessModifier(AccessModifier.Public)
                .UseMethodModifier(MethodModifier.Static));

    public IMethodBuilder AsInterfaceMethod() => _isInterfaceMethod.As();

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in PropagateDiagnostics(Modifier.Validate()))
            yield return diag;

        if (ReturnType is not null)
            foreach (var diag in PropagateDiagnostics(ReturnType.Validate()))
                yield return diag;

        foreach (var diag in _name.Validate(MethodName, nameof(MethodName)))
            yield return diag;

        foreach (var diag in _expression.Validate(Expression, Body, IsInterfaceMethod))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Parameters.SelectMany(parameter => parameter.Validate())))
            yield return diag;

        foreach (var diag in PropagateDiagnostics(Generics.Validate()))
            yield return diag;

        if (IsInterfaceMethod && Modifier.AccessModifier != AccessModifier.Public)
            yield return new InvalidBuilderConfigurationDiagnostic(this, $"{nameof(AccessModifier)} must be public for interface methods.");

        if (IsExtensionMethod)
        {
            foreach (var diag in _extensionMethodParameterName.Validate(ExtensionMethodParameterName, nameof(ExtensionMethodParameterName)))
                yield return diag;

            if (ReturnType is null)
                yield return new InvalidBuilderConfigurationDiagnostic(this, $"{nameof(ReturnType)} is required for extension methods");

            if (Modifier.AccessModifier != AccessModifier.Public)
                yield return new InvalidBuilderConfigurationDiagnostic(this, "Extension methods must be public");

            if (Modifier.MethodModifier != MethodModifier.Static)
                yield return new InvalidBuilderConfigurationDiagnostic(this, "Extension methods must be static");
        }
    }

    public void BuildContract(ICompilationStringBuilder icsb)
    {
        BuildMethodSignature(icsb);
        icsb.EnsureSemicolon();
    }

    public void BuildImplementation(ICompilationStringBuilder icsb)
    {
        Modifier.Build(icsb);
        icsb.EnsureSpace();
        BuildMethodSignature(icsb);

        if (Expression is not null)
        {
            Expression.Build(icsb, true);
        }
        else
        {
            Body!.Build(icsb);
        }

        icsb.AppendLine();
    }

    public IStatementParameterBuilder BuildMethodAsParameter()
    {
        if (ReturnType is null)
            throw new InvalidOperationException("Method must have a return type to be used as a parameter.");

        return new StatementParameterBuilder()
            .UseType(ReturnType)
            .ConfigureStatement(s => s.UseStatement($"{MethodName}({string.Join(", ", Parameters.Select(p => p.ParameterName))})"));
    }

    private void BuildMethodSignature(ICompilationStringBuilder icsb)
    {
        string separator = ", ";
        if (ReturnType is not null)
            ReturnType.BuildType(icsb);
        else
            icsb.Append("void");

        icsb.EnsureSpace()
            .Append(MethodName);

        Generics.BuildDeclaration(icsb);

        icsb.Append("(");
        if (IsValidExtensionMethod)
        {
            icsb.Append("this ");
            ReturnType.BuildType(icsb);
            icsb.EnsureSpace()
                .Append(ExtensionMethodParameterName);

            if (Parameters.Any())
                icsb.Append(separator);
        }
            
        icsb.AppendWithSeparator(separator, Parameters.Select(parameter => new StatementBuilder().UseStatement(parameter.Build)))
            .Append(")");

        if (Generics.Arguments.Any())
        {
            icsb.AppendLine();
            icsb.IncreaseIndent();
            Generics.BuildConstraints(icsb);
            icsb.DecreaseIndent();
        }
    }
}
