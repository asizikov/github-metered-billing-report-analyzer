using ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;

public class ActionsConsumptionReportSectionBuilder: IConsumptionReportSectionBuilder
{
    public decimal Build(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Actions consumption per organization");

        var totalConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.ActionsConsumptionPerOwner)
        {
            var totalCostForThisOwner = 0M;
            outputWriter.WriteTitle(3, owner.Key);
            outputWriter.WriteTitle(4, "Consumption per SKU");

            outputWriter.BeginTable("SKU", "Minutes", "Total cost");
            foreach (var sku in owner.Value.ConsumptionPerSku)
            {
                var costForThisSku = sku.Value.cost;
                outputWriter.WriteTableRow(sku.Key, sku.Value.cost.ToString("N1"), costForThisSku.ToUsString());
                totalCostForThisOwner += costForThisSku;
            }

            outputWriter.EndTable();

            outputWriter.WriteLine($"Total cost for this organization: {totalCostForThisOwner.ToUsString()}");
            totalConsumptionForEnterprise += totalCostForThisOwner;

            outputWriter.WriteTitle(4, "Top 3 repositories by consumption");
            outputWriter.BeginTable("Repository", "Total cost");
            foreach (var repository in owner.Value.CostPerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }

            outputWriter.EndTable();
        }

        outputWriter.WriteLine($"Total consumption for the enterprise: {totalConsumptionForEnterprise.ToUsString()}");
        return totalConsumptionForEnterprise;
    }
}