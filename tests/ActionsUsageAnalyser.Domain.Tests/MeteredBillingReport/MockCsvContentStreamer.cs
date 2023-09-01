namespace ActionsUsageAnalyser.Domain.Tests.MeteredBillingReport;

public class MockCsvContentStreamer : ICsvContentStreamer
{
    private readonly string[] lines;

    public MockCsvContentStreamer(string[] lines)
    {
        this.lines = lines;
    }
    
    public async IAsyncEnumerable<string> GetLinesAsync(string filename)
    {
        foreach (var line in lines)
        {
            yield return line;
        }
    }
}