namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

public class SharedStorageConsumptionSectionBuilder : IConsumptionReportSectionBuilder
{
    public decimal Build(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Shared storage consumption per organization");

        var totalStorageConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.StorageConsumptionPerOwner)
        {
            var totalStoragePriceForThisOwner = owner.Value.AccumulatedCost;
            outputWriter.WriteTitle(3, owner.Key);

            outputWriter.WriteTitle(4, "Top 3 repositories by storage cost");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.CostPerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }

            outputWriter.EndTable();

            outputWriter.WriteLine($"Total storage cost for this organization: {totalStoragePriceForThisOwner.ToUsString()}");
            totalStorageConsumptionForEnterprise += totalStoragePriceForThisOwner;
        }

        outputWriter.WriteLine(string.Empty);
        outputWriter.WriteLine($"Total storage consumption for the enterprise: {totalStorageConsumptionForEnterprise.ToUsString()}");
        return totalStorageConsumptionForEnterprise;
    }
}