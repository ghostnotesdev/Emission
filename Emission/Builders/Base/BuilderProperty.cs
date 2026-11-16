using Emission.Validation;
using Microsoft.CodeAnalysis.CSharp;

namespace Emission.Builders;
public abstract class BuilderPropertyBase<TSource>(TSource source)
    where TSource : BuilderBase
{
    protected readonly TSource _source = source;
}

public class SimpleBuilderProperty<TSource, T>(TSource source, T value) : BuilderPropertyBase<TSource>(source)
    where TSource : BuilderBase
{
    public T Value { get; private set; } = value;

    public TSource Use(T value)
    {
        Value = value;
        return _source;
    }
}

public class BuilderProperty<TSource, TBuilder, TDefaultBuilder>(TSource source) : BuilderPropertyBase<TSource>(source)
    where TSource : BuilderBase
    where TBuilder : IBuilder
    where TDefaultBuilder : TBuilder, new()
{
    public TBuilder Builder { get; private set; } = new TDefaultBuilder();

    public TSource Configure(Action<TBuilder> configure)
    {
        configure(Builder);
        return _source;
    }

    public TSource Use(TBuilder builder)
    {
        Builder = builder;
        return _source;
    }
}

public class OptionalBuilderProperty<TSource, TBuilder, TDefaultBuilder>(TSource source) : BuilderPropertyBase<TSource>(source)
    where TSource : BuilderBase
    where TBuilder : IBuilder
    where TDefaultBuilder : TBuilder, new()
{
    public TBuilder? Builder { get; private set; } = default;

    public TSource Configure(Action<TBuilder> configure)
    {
        TDefaultBuilder builder = new();
        configure(builder);
        return Use(builder);
    }

    public TSource Use(TBuilder? builder)
    {
        Builder = builder;
        return _source;
    }
}

public class BuilderIdentifierProperty<TSource>(TSource source, string? value = null) : SimpleBuilderProperty<TSource, string>(source, value ?? string.Empty)
    where TSource : BuilderBase
{
    public IEnumerable<IDiagnostic> Validate(string value, string builderPropName)
    {
        if (string.IsNullOrWhiteSpace(value) || !SyntaxFacts.IsValidIdentifier(value))
            yield return new InvalidBuilderConfigurationDiagnostic(_source, $"{builderPropName} cannot be null or contain invalid characters");
    }
}

public class BuilderNamespaceProperty<TSource>(TSource source) : SimpleBuilderProperty<TSource, string>(source, string.Empty)
    where TSource : BuilderBase
{
    public IEnumerable<IDiagnostic> Validate(string value)
    {
        if (!IsValidNamespace(value))
            yield return new InvalidBuilderConfigurationDiagnostic(_source, $"Namespace cannot be null or contain invalid characters");
    }

    private static bool IsValidNamespace(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value
            .Split('.')
            .All(SyntaxFacts.IsValidIdentifier);
    }
}

public class BuilderSwitchProperty<TSource>(TSource source) : SimpleBuilderProperty<TSource, bool>(source, false)
    where TSource : BuilderBase
{
    public TSource As() => Use(true);
}

public class SimpleBuilderEnumerableProperty<TSource, T>(TSource source) : BuilderPropertyBase<TSource>(source)
    where TSource : BuilderBase
{
    protected List<T> _entries = [];

    public IEnumerable<T> Entries => _entries;

    public TSource Use(IEnumerable<T> entries)
    {
        _entries = [.. entries];
        return _source;
    }

    public TSource With(T entry)
    {
        _entries.Add(entry);
        return _source;
    }
}

public class BuilderIdentifierEnumerableProperty<TSource>(TSource source, string builderPropName) : SimpleBuilderEnumerableProperty<TSource, string>(source)
    where TSource : BuilderBase
{
    private readonly string _builderPropName = builderPropName;

    public IEnumerable<IDiagnostic> Validate(IEnumerable<string> values) => values.SelectMany(value => ValidateValue(value));

    private IEnumerable<IDiagnostic> ValidateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !SyntaxFacts.IsValidIdentifier(value))
            yield return new InvalidBuilderConfigurationDiagnostic(_source, $"{_builderPropName} cannot be null or contain invalid characters");
    }
}

public class BuilderEnumerableProperty<TSource, TBuilder, TDefaultBuilder>(TSource source, Action<TBuilder>? preconfigure = null) : SimpleBuilderEnumerableProperty<TSource, TBuilder>(source)
    where TSource : BuilderBase
    where TBuilder : IBuilder
    where TDefaultBuilder : TBuilder, new()
{
    private readonly Action<TBuilder>? _preconfigure = preconfigure;

    public new TSource Use(IEnumerable<TBuilder> entries)
    {
        _entries = [.. entries.Select(e =>
        {
            _preconfigure?.Invoke(e);
            return e;
        })];
        return _source;
    }

    public TSource With(Action<TBuilder> configure)
    {
        TDefaultBuilder builder = new();
        configure(builder);
        return With(builder);
    }

    public new TSource With(TBuilder builder)
    {
        _preconfigure?.Invoke(builder);
        return base.With(builder);
    }
}

public class BuilderExpressionProperty<TSource>(TSource source) : BuilderPropertyBase<TSource>(source)
    where TSource : BuilderBase
{
    private OptionalBuilderProperty<TSource, IExpressionBuilder, ExpressionBuilder> _expression = new(source);
    private OptionalBuilderProperty<TSource, IBodyExpressionBuilder, BodyExpressionBuilder> _body = new(source);

    public IExpressionBuilder? Expression => _expression.Builder;
    public IBodyExpressionBuilder? Body => _body.Builder;
    public bool IsExpression => Expression is not null && Body is null;
    public bool IsBody => Expression is null && Body is not null;

    public TSource ConfigureExpression(Action<IExpressionBuilder> configure)
    {
        _body.Use(null);
        return _expression.Configure(configure);
    }

    public TSource UseExpression(IExpressionBuilder builder)
    {
        _body.Use(null);
        return _expression.Use(builder);
    }

    public TSource ConfigureBody(Action<IBodyExpressionBuilder> configure)
    {
        _expression.Use(null);
        return _body.Configure(configure);
    }

    public TSource UseBody(IBodyExpressionBuilder builder)
    {
        _expression.Use(null);
        return _body.Use(builder);
    }

    public IEnumerable<IDiagnostic> Validate(IExpressionBuilder? expression, IBodyExpressionBuilder? body, bool bothOptional = false)
    {
        if (!bothOptional && expression is null && body is null)
            yield return new InvalidBuilderConfigurationDiagnostic(_source, $"One of {nameof(Expression)} or {nameof(Body)} must be configured.");

        if (expression is not null && body is not null)
            yield return new InvalidBuilderConfigurationDiagnostic(_source, $"{nameof(Expression)} and {nameof(Body)} cannot both be configured.");

        if (expression is not null)
            foreach (var diag in _source.PropagateDiagnostics(expression.Validate()))
                yield return diag;

        if (body is not null)
            foreach (var diag in _source.PropagateDiagnostics(body.Validate()))
                yield return diag;
    }
}

public class StatementBuilderProperty(StatementBuilder source) : BuilderPropertyBase<StatementBuilder>(source)
{
    public string? StringStatement { get; private set; }
    public CompilationExpression? DeferredStatement { get; private set; }
    public ICustomStatementBuilder? CustomStatement { get; private set; }

    public StatementBuilder ConfigureStatement<TStatementBuilder>(Action<TStatementBuilder> configure)
        where TStatementBuilder : ICustomStatementBuilder, new()
    {
        TStatementBuilder builder = new();
        configure(builder);
        return UseStatement(builder);
    }

    public StatementBuilder UseStatement(string statement)
    {
        StringStatement = statement;
        DeferredStatement = null;
        CustomStatement = null;
        return _source;
    }

    public StatementBuilder UseStatement(CompilationExpression statement)
    {
        DeferredStatement = statement;
        StringStatement = null;
        CustomStatement = null;
        return _source;
    }

    public StatementBuilder UseStatement(ICustomStatementBuilder statement)
    {
        CustomStatement = statement;
        StringStatement = null;
        DeferredStatement = null;
        return _source;
    }
}