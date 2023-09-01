using ActionsUsageAnalyser.Domain.InsightsActionsReport;

namespace ActionsUsageAnalyzer.Domain;


public abstract class BillingReportReader
{
    public static async IAsyncEnumerable<ActionsReportItem> ReadFromFileAsync(string filename)
    {
        var counter = 0;
        var separator = ';';
        await foreach (var line in File.ReadLinesAsync(filename))
        {
            if (line.Contains("sep="))
            {
                separator = line.Split('=')[1][0];
            }
            if (counter++ == 0) continue;
            var values = line.Split(separator);
            var reportItem = ActionsReportItem.FromCsv(values);
            if (reportItem is not null) yield return reportItem;
        }
    }
}