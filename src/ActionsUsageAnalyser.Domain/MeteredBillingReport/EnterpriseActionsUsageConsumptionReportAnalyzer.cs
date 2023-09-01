using System.Globalization;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class EnterpriseActionsUsageConsumptionReportAnalyzer : IReportAnalyzer
{
    public async Task AnalyzeAsync(string dataFilePath)
    {
        var pricePerSku = new Dictionary<string, (decimal multiplier, decimal price)>();
        var enterprise = new Enterprise();
        await foreach (var reportItem in MeteredBillingReportReader.ReadFromFileAsync(dataFilePath))
        {
            if (reportItem.Product != "Actions") continue;
            enterprise.ActionsConsumptionPerOwner.TryAdd(reportItem.Owner, new ActionsConsumption());
            enterprise.ActionsConsumptionPerOwner[reportItem.Owner].MinutesPerSku.TryAdd(reportItem.SKU, 0);
            enterprise.ActionsConsumptionPerOwner[reportItem.Owner].MinutesPerSku[reportItem.SKU] += reportItem.Quantity;
            enterprise.ActionsConsumptionPerOwner[reportItem.Owner].PricePerRepository.TryAdd(reportItem.RepositorySlug, 0);
            enterprise.ActionsConsumptionPerOwner[reportItem.Owner].PricePerRepository[reportItem.RepositorySlug] += reportItem.Quantity * reportItem.Multiplier * reportItem.PricePerUnit;

            pricePerSku.TryAdd(reportItem.SKU, (reportItem.Multiplier, reportItem.PricePerUnit));
        }


        Console.WriteLine("Actions SKUs for this enterprise:");
        foreach (var sku in pricePerSku)
        {
            Console.WriteLine($"{sku.Key} - {sku.Value.price.ToString("C", CultureInfo.GetCultureInfo("en-US"))} per minute, multiplier: {sku.Value.multiplier}");
        }

        Console.WriteLine($"{Environment.NewLine}Total number of organizations: {enterprise.ActionsConsumptionPerOwner.Count} {Environment.NewLine}");

        var totalConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.ActionsConsumptionPerOwner)
        {
            var totalPriceForThisOwner = 0M;
            Console.WriteLine($"{Environment.NewLine}Owner: {owner.Key}");
            Console.WriteLine($"{Environment.NewLine}Consumption per SKU:");
            foreach (var sku in owner.Value.MinutesPerSku)
            {
                var priceForThisSku = sku.Value * pricePerSku[sku.Key].price * pricePerSku[sku.Key].multiplier;
                Console.WriteLine($"SKU: {sku.Key} - {sku.Value:N1} minutes, total price: {priceForThisSku.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
                totalPriceForThisOwner += priceForThisSku;
            }


            // print top 5 repositories by price
            Console.WriteLine($"{Environment.NewLine}Top 3 repositories by consumption:");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                Console.WriteLine($"{repository.Key} : {repository.Value.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            }

            Console.WriteLine($"{Environment.NewLine}------------------");

            Console.WriteLine($"Total cost for this owner: {totalPriceForThisOwner.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            Console.WriteLine("======================================");
            totalConsumptionForEnterprise += totalPriceForThisOwner;
        }

        Console.WriteLine($"Total consumption for the enterprise: {totalConsumptionForEnterprise.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
    }
}