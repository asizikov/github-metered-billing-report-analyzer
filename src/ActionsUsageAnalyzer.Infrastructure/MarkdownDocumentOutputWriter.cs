using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Infrastructure;

public class MarkdownDocumentOutputWriter(string filePath) : IOutputWriter
{
    private readonly TextWriter writer = File.CreateText(filePath);

    public void WriteLine(string line)
    {
        writer.WriteLine(line);
    }

    public void WriteTitle(int level, string line)
    {
        writer.WriteLine();
        writer.WriteLine($"{new string('#', level)} {line}");
        writer.WriteLine();
    }

    public void BeginTable(params string[] headers)
    {
        writer.WriteLine($"| {string.Join(" | ", headers)} |");
        writer.WriteLine($"| {string.Join(" | ", headers.Select(_ => "---"))} |");
    }

    public void WriteTableRow(params string[] columns)
    {
        writer.WriteLine($"| {string.Join(" | ", columns)} |");
    }

    public void EndTable()
    {
        writer.WriteLine();
    }

    public void Dispose()
    {
        writer.Close();
        writer.Dispose();
    }
}