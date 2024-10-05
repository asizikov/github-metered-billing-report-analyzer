namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

public class SharedStorageEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.SharedStorage) return;

        var owner = item.Owner;
        var cost = item.PricePerUnit * item.Quantity;

        enterprise.StorageConsumptionPerOwner.TryAdd(owner, new());

        var storageConsumption = enterprise.StorageConsumptionPerOwner[owner];
        storageConsumption.AccumulatedCost += cost;
        storageConsumption.CostPerRepository.TryAdd(item.RepositorySlug, 0);
        storageConsumption.CostPerRepository[item.RepositorySlug] += cost;
    }
}