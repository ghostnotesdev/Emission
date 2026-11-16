using Emission.Validation;

namespace Emission.Builders;
public class EnumBuilder : TopLevelBuilderBase<EnumBuilder, IEnumBuilder>, IEnumBuilder
{
    private readonly BuilderProperty<EnumBuilder, IEnumModifierBuilder, EnumModifierBuilder> _modifier;
    private readonly BuilderSwitchProperty<EnumBuilder> _isFlags;
    private readonly BuilderIdentifierEnumerableProperty<EnumBuilder> _values;

    public virtual IEnumModifierBuilder ModifierBuilder => _modifier.Builder;
    public virtual bool IsFlags => _isFlags.Value;
    public virtual IEnumerable<string> Values => _values.Entries;

    public override string DiagnosticName => !string.IsNullOrWhiteSpace(Name) ? Name : "<unnamed enum>";

    public EnumBuilder()
    {
        _modifier = new(this);
        _isFlags = new(this);
        _values = new(this, "Enum value");
    }

    public IEnumBuilder AsFlags() => _isFlags.As();
    public IEnumBuilder ConfigureModifier(Action<IEnumModifierBuilder> configure) => _modifier.Configure(configure);
    public IEnumBuilder UseValues(IEnumerable<string> values) => _values.Use(values);
    public IEnumBuilder WithValue(string value) => _values.With(value);

    public override IEnumerable<IDiagnostic> Validate()
    {
        foreach (var diag in base.Validate())
            yield return diag;

        foreach (var diag in ModifierBuilder.Validate())
            yield return diag;

        foreach (var diag in _values.Validate(Values))
            yield return diag;
    }

    public void BuildEnum(ICompilationStringBuilder icsb)
    {
        ModifierBuilder.Build(icsb);
        icsb.EnsureSpace()
            .AppendLine($"enum {Name}")
            .AppendLine("{")
            .IncreaseIndent()
            .AppendLinesWithSeparator(",", Values
                .Select((value, index) => new StatementBuilder()
                .UseStatement(value + " = " + (IsFlags ? $"1 << {index}" : index.ToString()))
            ))
            .DecreaseIndent()
            .AppendLine("}");
    }
}
