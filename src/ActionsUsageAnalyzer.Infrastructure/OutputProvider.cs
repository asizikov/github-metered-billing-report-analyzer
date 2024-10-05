using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Infrastructure;

public class OutputProvider(string? filePath) : IOutputProvider
{
    public IOutputWriter GetOutputWriter()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return new ConsoleOutputWriter();
        }
        return new MarkdownDocumentOutputWriter(filePath);
    }
}