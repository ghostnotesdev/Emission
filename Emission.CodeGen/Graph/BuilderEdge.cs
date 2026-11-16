using Microsoft.CodeAnalysis;

namespace Emission.CodeGen.Graph;

public sealed record BuilderEdge(
    BuilderNode Parent,
    BuilderNode Child,
    IMethodSymbol Method,
    string MethodPrefix,
    string MethodSubject,
    bool IsAction);
