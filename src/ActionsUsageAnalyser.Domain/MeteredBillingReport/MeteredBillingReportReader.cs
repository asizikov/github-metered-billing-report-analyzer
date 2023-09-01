using System.Globalization;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class MeteredBillingReportItemParser : IReportItemParser<MeteredBillingReportItem>
{
    public MeteredBillingReportItem Parse(string[] values)
    {
        return new MeteredBillingReportItem
        {
            Date = DateTime.Parse(values[0]),
            Product = values[1],
            SKU = values[2],
            Quantity = decimal.Parse(values[3]),
            UnitType = values[4],
            PricePerUnit = decimal.Parse(values[5], CultureInfo.InvariantCulture),
            Multiplier = decimal.Parse(values[6], CultureInfo.InvariantCulture),
            Owner = values[7],
            RepositorySlug = values[8],
            Username = values[9],
            ActionsWorkflow = values[10],
            Notes = values[11]
        };
    }
}

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