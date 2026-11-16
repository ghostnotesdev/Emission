namespace Emission.Builders;
public interface ICompilationStringBuilder
{
    bool IsStartOfLine { get; }
    ushort IndentDepth { get; set; }
    char? GetLastChar();
    ICompilationStringBuilder AppendLine();
    ICompilationStringBuilder AppendLine(string value);
    ICompilationStringBuilder Append(string value);
    ICompilationStringBuilder IncreaseIndent();
    ICompilationStringBuilder DecreaseIndent();
    ICompilationStringBuilder EnsureSpace();
    ICompilationStringBuilder EnsureSemicolon();
    ICompilationStringBuilder EnsureNewLine();
    ICompilationStringBuilder AppendGeneratorHeader(string _namespace);
    ICompilationStringBuilder AppendLinesWithSeparator(string separator, IEnumerable<string> lines);
    ICompilationStringBuilder AppendLinesWithSeparator(string separator, params IStatementBuilder[] valueBuilders);
    ICompilationStringBuilder AppendLinesWithSeparator(string separator, IEnumerable<IStatementBuilder> valueBuilders);
    ICompilationStringBuilder AppendWithSeparator(string separator, IEnumerable<string> lines);
    ICompilationStringBuilder AppendWithSeparator(string separator, params IStatementBuilder[] valueBuilders);
    ICompilationStringBuilder AppendWithSeparator(string separator, IEnumerable<IStatementBuilder> valueBuilders);
}
