namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public interface IReportAnalyzer
{
    Task AnalyzeAsync(string reportPath);
}