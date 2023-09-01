namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public static class MeteredBillingReportReader
{
    public static async IAsyncEnumerable<MeteredBillingReportItem> ReadFromFileAsync(string filename)
    {
        var counter = 0;
        var separator = ';';
        await foreach (var line in File.ReadLinesAsync(filename))
        {
            if(counter == 0 && line.Contains("sep="))
            {
                separator = line.Split('=')[1][0];
                continue;
            }
            if (counter++ == 0) continue;
            var values = line.Split(separator);
            var reportItem = MeteredBillingReportItem.FromCsv(values);
            if (reportItem is not null) yield return reportItem;
        }
    }
}