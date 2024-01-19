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
        // return a HtmlDocumentOutputWriter if the output file path has a .html extension
        if (Path.GetExtension(filePath) == ".html")
        {
            return new HtmlDocumentOutputWriter(filePath);
        }
        return new MarkdownDocumentOutputWriter(filePath);
    }
}
