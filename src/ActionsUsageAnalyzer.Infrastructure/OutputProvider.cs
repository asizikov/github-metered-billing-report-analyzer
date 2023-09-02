using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Infrastructure;

public class OutputProvider : IOutputProvider
{
    private readonly string? filePath;

    public OutputProvider(string? filePath)
    {
        this.filePath = filePath;
    }
    
    public IOutputWriter GetOutputWriter()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return new ConsoleOutputWriter();
        }
        return new MarkdownDocumentOutputWriter(filePath);
    }
}