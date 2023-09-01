using ActionsUsageAnalyser.Domain.MeteredBillingReport;

var dataFilePath = @"/Users/asizikov/code/github/github-actions-usage-analyzer/data/august.csv";

Console.WriteLine($"Report {Path.GetFileName(dataFilePath)}");

var reportAnalyzer = new EnterpriseActionsUsageConsumptionReportAnalyzer();
await reportAnalyzer.AnalyzeAsync(dataFilePath);