namespace ActionsUsageAnalyzer.Domain;

public static class InsightsReportReader
{
    public static async IAsyncEnumerable<InternalActionsReportItem> ReadFromFileAsync(string filename)
    {
        var counter = 0;
        await foreach (var line in File.ReadLinesAsync(filename))
        {
            if (counter++ == 0) continue;
            var values = line.Split(';');
            var reportItem = InternalActionsReportItem.FromCsv(values);
            if (reportItem is not null) yield return reportItem;
        }
    }
}