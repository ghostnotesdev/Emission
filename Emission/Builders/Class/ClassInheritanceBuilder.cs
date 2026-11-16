using Emission.Validation;

namespace Emission.Builders;
public class ClassInheritanceBuilder : InheritanceBuilder<ClassInheritanceBuilder>, IClassInheritanceBuilder
{
    private readonly OptionalBuilderProperty<ClassInheritanceBuilder, ITypeBuilder, TypeBuilder> _baseClass;
    
    public virtual ITypeBuilder? BaseClass => _baseClass.Builder;

    public ClassInheritanceBuilder()
    {
        _baseClass = new(this);
    }

    public IClassInheritanceBuilder ConfigureBaseClass(Action<ITypeBuilder> configure) => _baseClass.Configure(configure);
    public IClassInheritanceBuilder UseBaseClass(ITypeBuilder builder) => _baseClass.Use(builder);
    public IClassInheritanceBuilder UseImplementations(IEnumerable<ITypeBuilder> builders) => UseTypes(builders);
    public IClassInheritanceBuilder WithImplementation(Action<ITypeBuilder> configure) => WithType(configure);
    public IClassInheritanceBuilder WithImplementation(ITypeBuilder builder) => WithType(builder);

    public override IEnumerable<IDiagnostic> Validate()
    {
        if (BaseClass is not null)
        {
            foreach (var diag in PropagateDiagnostics(BaseClass.Validate()))
                yield return diag;
        }
    }

    public override void Build(ICompilationStringBuilder icsb)
    {
        if (BaseClass is null)
        {
            base.Build(icsb);
            return;
        }

        IEnumerable<IStatementBuilder> types =
        [
            new StatementBuilder().UseStatement(BaseClass.BuildType),
            .. Types.Select(t => new StatementBuilder().UseStatement(t.BuildType))
        ];

        icsb.EnsureSpace()
            .Append(": ")
            .AppendWithSeparator(", ", types);
    }
}
