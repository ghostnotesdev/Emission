using Emission.Builders;
using Emission.CodeGen;

GeneratorRunner provider = new();
foreach (BuiltSource src in provider.Run())
{
    SourceWriter.WriteSource(Path.Combine("Emission", "Builders", "Extensions"), src);
}
