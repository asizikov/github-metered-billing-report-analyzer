namespace ActionsUsageAnalyser.Domain;

public interface ICsvContentStreamer
{
    IAsyncEnumerable<string> GetLinesAsync(string filename);
}