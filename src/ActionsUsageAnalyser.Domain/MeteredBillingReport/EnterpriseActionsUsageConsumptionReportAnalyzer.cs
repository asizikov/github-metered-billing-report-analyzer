using Microsoft.Extensions.Logging;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class EnterpriseActionsUsageConsumptionReportAnalyzer(
    IReportReader<MeteredBillingReportItem> reportReader,
    IOutputProvider outputProvider,
    ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger)
    : IReportAnalyzer
{
    private readonly IReportReader<MeteredBillingReportItem> reportReader = reportReader ?? throw new ArgumentNullException(nameof(reportReader));
    private readonly IOutputProvider outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
    private readonly ILogger<EnterpriseActionsUsageConsumptionReportAnalyzer> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    private readonly Dictionary<Product, IReportEntryDataProcessor> dataProcessors = new()
    {
        { Product.Actions, new ActionsEntryDataProcessor() },
        { Product.Copilot, new CopilotEntryDataProcessor() },
        { Product.SharedStorage, new SharedStorageEntryDataProcessor()},
        { Product.Packages, new PackagesEntryDataProcessor()}
    };

    public async Task AnalyzeAsync(string dataFilePath)
    {
        try
        {
            var pricePerSku = new Dictionary<string, (decimal multiplier, string unit, decimal price)>();
            var enterprise = new Enterprise();
            await foreach (var reportItem in reportReader.ReadFromSourceAsync(dataFilePath))
            {
                if (!dataProcessors.TryGetValue(reportItem.Product, out var dataProcessor)) continue;
                dataProcessor.ProcessForEnterprise(enterprise, reportItem);
                pricePerSku.TryAdd(reportItem.SKU, (reportItem.Multiplier, reportItem.UnitType, reportItem.PricePerUnit));
            }

            using var outputWriter = outputProvider.GetOutputWriter();

            outputWriter.WriteTitle(2, "Metered SKUs for this enterprise");

            outputWriter.BeginTable("SKU","Unit", "Price per unit", "Multiplier");
            foreach (var sku in pricePerSku.OrderBy(kv => kv.Key))
            {
                outputWriter.WriteTableRow(sku.Key, sku.Value.unit, sku.Value.price.ToUsString(), sku.Value.multiplier.ToString("N1"));
            }
            outputWriter.EndTable();

            outputWriter.WriteLine($"Total number of organizations: {enterprise.ActionsConsumptionPerOwner.Count}");

            var actionsCost = BuildActionsConsumptionSection(outputWriter, enterprise, pricePerSku);
            var storageCost = BuildSharedStorageConsumptionSection(outputWriter, enterprise);
            var packagesCost = BuildPackagesConsumptionSecction(outputWriter, enterprise);
            var copilotCost =  BuildCopilotConsumptionSecction(outputWriter, enterprise);
            
            outputWriter.WriteTitle(2, "Summary for this enterprise");
            
            outputWriter.BeginTable("Metered Cost", "Total price");
            outputWriter.WriteTableRow("Actions", actionsCost.ToUsString());
            outputWriter.WriteTableRow("Shared storage", storageCost.ToUsString());
            outputWriter.WriteTableRow("Packages", packagesCost.ToUsString());
            outputWriter.WriteTableRow("Copilot", copilotCost.ToUsString());
            outputWriter.EndTable();
            
            outputWriter.WriteLine("Grand total: " + (actionsCost + packagesCost + storageCost + copilotCost).ToUsString());
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to process file {dataFilePath}");
        }
    }

    private static decimal BuildActionsConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise, Dictionary<string, (decimal multiplier, string unit, decimal price)> pricePerSku)
    {
        outputWriter.WriteTitle(2, "Actions consumption per organization");

        var totalConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.ActionsConsumptionPerOwner)
        {
            var totalPriceForThisOwner = 0M;
            outputWriter.WriteTitle(3, owner.Key);
            outputWriter.WriteTitle(4, "Consumption per SKU");

            outputWriter.BeginTable("SKU", "Minutes", "Total price");
            foreach (var sku in owner.Value.MinutesPerSku)
            {
                var priceForThisSku = sku.Value * pricePerSku[sku.Key].price;
                outputWriter.WriteTableRow(sku.Key, sku.Value.ToString("N1"), priceForThisSku.ToUsString());
                totalPriceForThisOwner += priceForThisSku;
            }
            outputWriter.EndTable();


            outputWriter.WriteLine($"Total cost for this organization: {totalPriceForThisOwner.ToUsString()}");
            totalConsumptionForEnterprise += totalPriceForThisOwner;

            outputWriter.WriteTitle(4, "Top 3 repositories by consumption");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
        }

        outputWriter.WriteLine($"Total consumption for the enterprise: {totalConsumptionForEnterprise.ToUsString()}");
        return totalConsumptionForEnterprise;
    }

    private static decimal BuildSharedStorageConsumptionSection(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Shared storage consumption per organization");
            
        var totalStorageConsumptionForEnterprise = 0M;
        foreach (var owner in enterprise.StorageConsumptionPerOwner)
        {
            var totalStoragePriceForThisOwner = owner.Value.AccumulatedPrice;
            outputWriter.WriteTitle(3, owner.Key);
            
            outputWriter.WriteTitle(4, "Top 3 repositories by storage cost");
            outputWriter.BeginTable("Repository", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
            
            outputWriter.WriteLine($"Total storage cost for this organization: {totalStoragePriceForThisOwner.ToUsString()}");
            totalStorageConsumptionForEnterprise += totalStoragePriceForThisOwner;
        }
        outputWriter.WriteLine($"Total storage consumption for the enterprise: {totalStorageConsumptionForEnterprise.ToUsString()}");
        return totalStorageConsumptionForEnterprise;
    }

    private static decimal BuildCopilotConsumptionSecction(IOutputWriter outputWriter, Enterprise enterprise)
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
        outputWriter.WriteLine($"Total copilot consumption for the enterprise: {totalCopilotConsumptionForEnterprise.ToUsString()}");
        return totalCopilotConsumptionForEnterprise;
    }

    private static decimal BuildPackagesConsumptionSecction(IOutputWriter outputWriter, Enterprise enterprise)
    {
        outputWriter.WriteTitle(2, "Packages consumption per organization");
            
        var totalPackagesConsumptionForEnterprise = 0M;

        foreach (var owner in enterprise.PackagesConsumptionPerOwner)
        {
            var totalPackagesPriceForThisOwner = owner.Value.AccumulatedCost;
                
            outputWriter.WriteTitle(3, owner.Key);

            outputWriter.WriteTitle(4, "Top 3 sources by packages cost");
            outputWriter.BeginTable("Source", "Total price");
            foreach (var repository in owner.Value.PricePerRepository.OrderByDescending(x => x.Value).Take(3))
            {
                outputWriter.WriteTableRow(repository.Key, repository.Value.ToUsString());
            }
            outputWriter.EndTable();
                
            outputWriter.WriteLine($"Total packages cost for this organization: {totalPackagesPriceForThisOwner.ToUsString()}");
            totalPackagesConsumptionForEnterprise += totalPackagesPriceForThisOwner;
        }
        outputWriter.WriteLine($"Total packages consumption for the enterprise: {totalPackagesConsumptionForEnterprise.ToUsString()}");
        return totalPackagesConsumptionForEnterprise;
    }
}