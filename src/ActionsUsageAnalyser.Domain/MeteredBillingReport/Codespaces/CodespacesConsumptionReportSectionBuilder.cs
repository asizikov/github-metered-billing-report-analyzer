namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Codespaces;

public static class CodespacesConsumptionReportSectionBuilder
{
    public static decimal Build(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Codespaces consumption per organization");

        var totalCodespacesConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.CodespacesConsumptionPerOwner)
        {
            var totalCodespacesPriceForThisOwner = owner.Value.AccumulatedCost;
            outputWriter.WriteTitle(3, owner.Key);

            outputWriter.WriteTitle(4, "Consumption per SKU");
            outputWriter.BeginTable("SKU", "Unit", "Total cost");
            foreach (var sku in owner.Value.ConsumptionPerSku)
            {
                outputWriter.WriteTableRow(sku.Key, sku.Value.unit, sku.Value.cost.ToUsString());
            }

            outputWriter.EndTable();

            outputWriter.WriteTitle(4, "Top 3 repositories by codespaces cost");
            outputWriter.BeginTable("Repository", "Total cost");
            foreach (var repository in owner.Value.CostPerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }

            outputWriter.EndTable();

            outputWriter.WriteLine($"Total codespaces cost for this organization: {totalCodespacesPriceForThisOwner.ToUsString()}");
            totalCodespacesConsumptionForEnterprise += totalCodespacesPriceForThisOwner;
        }

        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total codespaces consumption for the enterprise: {totalCodespacesConsumptionForEnterprise.ToUsString()}");
        return totalCodespacesConsumptionForEnterprise;
    }
}