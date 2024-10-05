namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;

public static class ActionsConsumptionReportSectionBuilder
{
    public static decimal Build(IOutputWriter outputWriter, Enterprise enterprise, Dictionary<string, (Product product, decimal multiplier, string unit, decimal price)> pricePerSku)
    {
        outputWriter.WriteTitle(2, "Actions consumption per organization");

        var totalConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.ActionsConsumptionPerOwner)
        {
            var totalPriceForThisOwner = 0M;
            outputWriter.WriteTitle(3, owner.Key);
            outputWriter.WriteTitle(4, "Consumption per SKU");

            outputWriter.BeginTable("SKU", "Minutes", "Total cost");
            foreach (var sku in owner.Value.MinutesPerSku)
            {
                var costForThisSku = sku.Value * pricePerSku[sku.Key].price;
                outputWriter.WriteTableRow(sku.Key, sku.Value.ToString("N1"), costForThisSku.ToUsString());
                totalPriceForThisOwner += costForThisSku;
            }

            outputWriter.EndTable();

            outputWriter.WriteLine($"Total cost for this organization: {totalPriceForThisOwner.ToUsString()}");
            totalConsumptionForEnterprise += totalPriceForThisOwner;

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