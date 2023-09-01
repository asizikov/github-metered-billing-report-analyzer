using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Cli.Infrastructure;

public class FileContentStreamer : ICsvContentStreamer
{
    public IAsyncEnumerable<string> GetLinesAsync(string filename) 
        => File.ReadLinesAsync(filename);
}