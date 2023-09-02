using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Infrastructure;

public class FileContentStreamer : ICsvContentStreamer
{
    public IAsyncEnumerable<string> GetLinesAsync(string filename) 
        => File.ReadLinesAsync(filename);
}