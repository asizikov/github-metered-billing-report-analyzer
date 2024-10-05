namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

public interface IConsumptionReportSectionBuilder
{
    decimal Build(IOutputWriter outputWriter, Enterprise enterprise);
}