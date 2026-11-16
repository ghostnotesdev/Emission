using Emission.Builders;
using Emission.CodeGen.Builders;
using Emission.CodeGen.Graph;
using Emission.CodeGen.Types;
using Emission.Context;
using Emission.Generators;
using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Generators;

public sealed class ExtensionsGenerator : GeneratorBase<
    GeneratorContext<CodeGenTypeResolver, ICodeGenTypeResolver>,
    IGeneratorContext<ICodeGenTypeResolver>,
    ICodeGenTypeResolver>
{
    private Dictionary<string, ExtensionClassBuilder> _extensions = [];

    protected override void Execute(IGeneratorContext<ICodeGenTypeResolver> context)
    {
        EmissionGraph graph = new(context.TypeResolver.BuilderNamespace, context.TypeResolver.IBuilder);
        AddTypeBuilderExtensions(context, graph);
        AddStatementBuilderExtensions(context, graph);
        BuildExtensions(context);
    }

    private void AddTypeBuilderExtensions(IGeneratorContext<ICodeGenTypeResolver> context, EmissionGraph graph)
    {
        foreach (BuilderEdge edge in graph.GetConsumers(context.TypeResolver.ITypeBuilder))
        {
            if (edge.IsAction)
                continue;

            AddMethodsToExtensionBuilder(edge.Parent.Interface.Name, TypeBuilderExtensionMethodBuilder.GetAll(
                context.TypeResolver,
                edge.Method.Name,
                edge.Parent.Interface));
        }
    }

    private void AddStatementBuilderExtensions(IGeneratorContext<ICodeGenTypeResolver> context, EmissionGraph graph)
    {
        INamedTypeSymbol statementBuilder = context.TypeResolver.IStatementBuilder;
        AddMethodsToExtensionBuilder(statementBuilder.Name, CustomStatementBuilderExtensionMethodBuilder.GetAll(context.TypeResolver, context.TypeResolver.IStatementBuilder));

        foreach (BuilderEdge edge in graph.GetConsumers(statementBuilder))
        {
            if (edge.Parent.Interface.IsGenericType)
            {
                foreach (var implementation in edge.Parent.Implementations)
                {
                    GenerateStatementBuilderExtensions(context, new BuilderEdge(
                        implementation,
                        edge.Child,
                        edge.Method,
                        edge.MethodPrefix,
                        edge.MethodSubject,
                        edge.IsAction));

                    if (edge.MethodPrefix == "Use" || (edge.MethodPrefix == "With" && edge.IsAction))
                    {
                        AddMethodsToExtensionBuilder(implementation.Interface.Name, CustomStatementBuilderExtensionMethodBuilder.GetAll(
                            context.TypeResolver,
                            implementation.Interface,
                            edge.Method.Name));
                    }
                }
            }
            else
            {
                GenerateStatementBuilderExtensions(context, edge);

                if (edge.MethodPrefix == "Use" || (edge.MethodPrefix == "With" && edge.IsAction))
                {
                    AddMethodsToExtensionBuilder(edge.Parent.Interface.Name, CustomStatementBuilderExtensionMethodBuilder.GetAll(
                        context.TypeResolver,
                        edge.Parent.Interface,
                        edge.Method.Name));
                }
            }

            foreach (BuilderEdge parentEdge in graph.GetConsumers(edge.Parent.Interface))
            {
                if (parentEdge.Parent.Interface.IsGenericType)
                {
                    foreach (var implementation in parentEdge.Parent.Implementations)
                    {
                        GenerateStatementBuilderExtensions(context, edge, new BuilderEdge(
                            implementation,
                            parentEdge.Child,
                            parentEdge.Method,
                            parentEdge.MethodPrefix,
                            parentEdge.MethodSubject,
                            parentEdge.IsAction));
                    }
                }
                else
                {
                    GenerateStatementBuilderExtensions(context, edge, parentEdge);
                }
            }
        }
    }

    private void GenerateStatementBuilderExtensions(IGeneratorContext<ICodeGenTypeResolver> context, BuilderEdge edge, BuilderEdge? parentEdge = null)
    {
        var stringType = context.TypeResolver.String;
        var compilationExpression = context.TypeResolver.CompilationExpression;
        var customStatmentBuilder = context.TypeResolver.ICustomStatementBuilder;
        if (parentEdge is not null)
        {
            var builderType = new TypeBuilder().UseType(parentEdge.Parent.Interface);
            if (edge.IsAction && parentEdge.IsAction)
            {
                AddMethodsToExtensionBuilder(parentEdge.Parent.Interface.Name, new MethodBuilder()
                    .UseReturnType(builderType)
                    .UseName(parentEdge.Method.Name)
                    .UseExtensionMethod("builder")
                    .WithParameter(paramBuilder => paramBuilder
                        .ConfigureType(type => type.UseType(edge.Method.Parameters[0].Type))
                        .UseName("configure"))
                    .ConfigureExpression(expression => expression
                        .ConfigureExpression(statement => statement
                            .UseStatement($"builder.{parentEdge.Method.Name}(x => x.{edge.Method.Name}(configure))"))));
            }
            else if (!edge.IsAction && !parentEdge.IsAction)
            {
                var prefix = parentEdge.MethodPrefix == "Use" ? "Configure" : parentEdge.MethodPrefix;
                AddMethodsToExtensionBuilder(parentEdge.Parent.Interface.Name, new MethodBuilder()
                    .UseReturnType(builderType)
                    .UseName(parentEdge.Method.Name)
                    .UseExtensionMethod("builder")
                    .WithParameter(paramBuilder => paramBuilder
                        .ConfigureType(type => type.UseType(context.TypeResolver.IStatementBuilder))
                        .UseName("statement"))
                    .ConfigureExpression(expression => expression
                        .ConfigureExpression(statement => statement
                            .UseStatement($"builder.{prefix}{parentEdge.MethodSubject}(x => x.{edge.Method.Name}(statement))"))));

                AddMethodsToExtensionBuilder(parentEdge.Parent.Interface.Name, StatementBuilderExtensionMethodBuilder.GetAll(
                    context.TypeResolver,
                    parentEdge.Parent.Interface,
                    parentEdge.Method.Name));
            }
        }
        else if (!edge.IsAction)
        {
            AddMethodsToExtensionBuilder(edge.Parent.Interface.Name, StatementBuilderExtensionMethodBuilder.GetAll(
                context.TypeResolver,
                edge.Parent.Interface,
                edge.Method.Name));
        }
    }

    private void AddMethodsToExtensionBuilder(string builderInterfaceName, params IMethodBuilder[] methods)
    {
        ExtensionClassBuilder builder;
        if (_extensions.TryGetValue(builderInterfaceName, out ExtensionClassBuilder? value))
            builder = value;
        else
        {
            builder = new ExtensionClassBuilder(builderInterfaceName);
            _extensions.Add(builderInterfaceName, builder);
        }

        foreach (IMethodBuilder method in methods)
            builder.WithMethod(method);
    }

    private void BuildExtensions(IGeneratorContext<ICodeGenTypeResolver> context)
    {
        foreach (var extension in _extensions.Values)
        {
            context.AddClassSource(extension);
        }

        context.BuildSource();
    }
}
