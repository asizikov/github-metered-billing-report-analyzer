namespace ActionsUsageAnalyser.Domain.Tests.MeteredBillingReport;

public class MockCsvContentStreamer(string[] lines) : ICsvContentStreamer
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async IAsyncEnumerable<string> GetLinesAsync(string filename)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (var line in lines)
        {
            yield return line;
        }
    }
}