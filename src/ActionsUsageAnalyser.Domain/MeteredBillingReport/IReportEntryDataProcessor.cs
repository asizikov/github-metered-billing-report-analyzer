namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public interface IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item);
}