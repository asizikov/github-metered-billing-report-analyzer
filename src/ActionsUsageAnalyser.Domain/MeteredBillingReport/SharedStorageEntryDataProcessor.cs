namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class SharedStorageEntryDataProcessor: IReportEntryDataProcessor
{
    
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.SharedStorage) return;

        var owner = item.Owner;
        var price = item.PricePerUnit * item.Quantity;

        enterprise.StorageConsumptionPerOwner.TryAdd(owner, new());

        var storageConsumption = enterprise.StorageConsumptionPerOwner[owner];
        storageConsumption.AccumulatedPrise += price;
    }
}