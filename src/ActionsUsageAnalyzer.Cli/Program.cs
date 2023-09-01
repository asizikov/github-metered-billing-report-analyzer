using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using ActionsUsageAnalyzer.Cli.Infrastructure;

var dataFilePath = @"/Users/asizikov/code/github/github-actions-usage-analyzer/data/august.csv";

Console.WriteLine($"Report {Path.GetFileName(dataFilePath)}");

var reportAnalyzer = new EnterpriseActionsUsageConsumptionReportAnalyzer(new MeteredBillingReportReader(new FileContentStreamer(), new MeteredBillingReportItemParser()), 
    new ConsoleOutputWriter()
    //new MarkdownDocumentOutputWriter(@"/Users/asizikov/code/github/github-actions-usage-analyzer/output/result.md")
    );
await reportAnalyzer.AnalyzeAsync(dataFilePath);