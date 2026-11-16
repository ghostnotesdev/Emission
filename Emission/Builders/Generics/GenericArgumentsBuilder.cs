using Emission.Validation;

namespace Emission.Builders;
public class GenericArgumentsBuilder : BuilderBase, IGenericArgumentsBuilder
{
    private readonly BuilderEnumerableProperty<GenericArgumentsBuilder, IGenericArgumentBuilder, GenericArgumentBuilder> _arguments;
    public virtual IEnumerable<IGenericArgumentBuilder> Arguments => _arguments.Entries;

    public GenericArgumentsBuilder()
    {
        _arguments = new(this);
    }

    public IGenericArgumentsBuilder UseArguments(IEnumerable<IGenericArgumentBuilder> builders) => _arguments.Use(builders);
    public IGenericArgumentsBuilder WithArgument(Action<IGenericArgumentBuilder> configure) => _arguments.With(configure);
    public IGenericArgumentsBuilder WithArgument(IGenericArgumentBuilder builder) => _arguments.With(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        HashSet<string> names = [];

        foreach (IGenericArgumentBuilder argument in Arguments)
        {
            foreach (var diag in PropagateDiagnostics(argument.Validate()))
                yield return diag;

            if (!string.IsNullOrWhiteSpace(argument.ArgumentName) && !names.Add(argument.ArgumentName!))
            {
                yield return new InvalidBuilderConfigurationDiagnostic(this, $"Generic argument '{argument.ArgumentName}' is declared more than once.");
            }
        }
    }

    public void BuildDeclaration(ICompilationStringBuilder icsb)
    {
        if (!Arguments.Any())
            return;

        icsb.Append("<")
            .AppendWithSeparator(", ", Arguments.Select(argument => new StatementBuilder().UseStatement(argument.BuildDeclaration)))
            .Append(">");
    }

    public void BuildConstraints(ICompilationStringBuilder builder)
    {
        foreach (IGenericArgumentBuilder argument in Arguments)
            argument.BuildConstraints(builder);
    }
}
