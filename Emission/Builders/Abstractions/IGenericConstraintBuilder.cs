namespace Emission.Builders;
public interface IGenericConstraintBuilder : IBuilder
{
    GenericPrimaryConstraint PrimaryConstraint { get; }
    bool HasConstructorConstraint { get; }
    bool HasConstraints { get; }

    IGenericConstraintBuilder UseConstraint(GenericPrimaryConstraint constraint);
    IGenericConstraintBuilder WithImplementation(Action<ITypeBuilder> configure);
    IGenericConstraintBuilder WithImplementation(ITypeBuilder builder);
    IGenericConstraintBuilder AsParameterlessConstructor();
    void Build(ICompilationStringBuilder icsb);
}
