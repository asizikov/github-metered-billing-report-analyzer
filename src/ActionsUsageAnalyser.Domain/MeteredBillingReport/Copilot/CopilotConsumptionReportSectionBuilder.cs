using ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Copilot;

public class CopilotConsumptionReportSectionBuilder: IConsumptionReportSectionBuilder
{
    public decimal Build(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Copilot consumption per organization");

        var totalCopilotConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.CopilotConsumptionPerOwner)
        {
            var totalCopilotPriceForThisOwner = owner.Value.AccumulatedCost;
            outputWriter.WriteTitle(3, owner.Key);

            outputWriter.WriteLine($"Total copilot cost for this organization: {totalCopilotPriceForThisOwner.ToUsString()}");
            totalCopilotConsumptionForEnterprise += totalCopilotPriceForThisOwner;
        }

        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total copilot consumption for the enterprise: {totalCopilotConsumptionForEnterprise.ToUsString()}");
        return totalCopilotConsumptionForEnterprise;
    }
}