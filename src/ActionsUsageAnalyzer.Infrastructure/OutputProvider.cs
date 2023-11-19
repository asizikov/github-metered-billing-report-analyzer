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
        else if (filePath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
        {
            return new HtmlOutputWriter(filePath);
        }
        else
        {
            return new MarkdownDocumentOutputWriter(filePath);
        }
    }
}