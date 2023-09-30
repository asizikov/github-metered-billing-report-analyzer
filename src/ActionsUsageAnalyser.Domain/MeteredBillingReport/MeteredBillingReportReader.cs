namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class MeteredBillingReportReader: IReportReader<MeteredBillingReportItem>
{
    private readonly ICsvContentStreamer fileContentStreamer;
    private readonly IReportItemParser<MeteredBillingReportItem> reportItemParser;

    public MeteredBillingReportReader(ICsvContentStreamer fileContentStreamer, IReportItemParser<MeteredBillingReportItem> reportItemParser)
    {
        this.fileContentStreamer = fileContentStreamer;
        this.reportItemParser = reportItemParser;
    }

    public async IAsyncEnumerable<MeteredBillingReportItem> ReadFromSourceAsync(string filename)
    {
        var counter = 0;
        var separator = ',';
        await foreach (var line in fileContentStreamer.GetLinesAsync(filename))
        {
            if(counter == 0 && line.Contains("sep="))
            {
                separator = line.Split('=')[1][0];
                continue;
            }
            if (counter++ == 0) continue;
            var values = line.Split(separator);
            var reportItem = reportItemParser.Parse(values);
            if (reportItem is not null) yield return reportItem;
        }
    }
}